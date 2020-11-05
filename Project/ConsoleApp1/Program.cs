using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;
using ConfigStorageSP;
using Grpc.Core;
using Newtonsoft.Json.Linq;

namespace ServerSP
{

    class ServerServices : ServerService.ServerServiceBase
    {

        Dictionary<(string, string), string> dataStorage = new Dictionary<(string, string), string>();

        string id;
        int mindelay;
        int maxdelay;


        public ServerServices(string id, int mindelay, int maxdelay) {
            this.id = id;
            this.mindelay = mindelay;
            this.maxdelay = maxdelay;
        }



        public override Task<ReadReply> Read(ReadRequest request, ServerCallContext context)
        {
            Thread.Sleep((new Random()).Next(mindelay, maxdelay));
            string res;
            lock (this)
            {
                try
                {
                    res = dataStorage[(request.PartitionId, request.ObjectId)];
                } catch(KeyNotFoundException)
                {
                    res = "N/A";
                }
            }

            
            return Task.FromResult(new ReadReply
            {
                ObjectValue = res
            });
        }

        public override Task<WriteReply> Write(WriteRequest request, ServerCallContext context)
        {

            Thread.Sleep((new Random()).Next(mindelay, maxdelay));
            lock (this)
            {
                dataStorage[(request.PartitionId, request.ObjectId)] = request.ObjectValue;
            }
                        
            return Task.FromResult(new WriteReply { Ok = true });
        }

    }

    class Program
    {

        public static void Main(string[] args)
        {

            string id = args[0];
            Uri uri = new Uri(args[1]);
            int mindelay = Int32.Parse(args[2]);
            int maxdelay = Int32.Parse(args[3]);

            Server server = new Server
            {
                Services = { ServerService.BindService(new ServerServices( id, mindelay , maxdelay)) },
                Ports = { new ServerPort(uri.Host, uri.Port, ServerCredentials.Insecure) }
            };

            server.Start();
            Console.ReadKey();
            server.ShutdownAsync().Wait();

        }
        
    }
}
