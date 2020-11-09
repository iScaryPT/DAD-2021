using System;
using System.Collections;
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
    class ServerInfo
    {
        private string name;
        private string url;
        private List<string> master;
        private List<string> partitions;
        private int mindelay;
        private int maxdelay;
        public string Name { get { return name; } }
        public string Url { get { return url; } }
        public List<string> Master { get { return master; } }
        public List<string> Partitions { get { return partitions; } }
        public int Mindelay { get { return mindelay; } }
        public int Maxdelay { get { return maxdelay; } }

        public ServerInfo(string info)
        {
            string[] ser = info.Split("|");
            this.name = ser[0];
            this.url = ser[1];
            this.master = new List<string>(ser[2].Split(" "));
            this.partitions = new List<string>(ser[3].Split(" "));
            this.mindelay = Int32.Parse(ser[4]);
            this.maxdelay = Int32.Parse(ser[5]);
        }

        public override string ToString()
        {
            Console.WriteLine("[ToString] start");
            Console.WriteLine(name);
            Console.WriteLine(url);
            Console.WriteLine("Master: ");
            foreach (string m in master)
            {
                Console.WriteLine(m);
            }
            Console.WriteLine("Partitions: ");
            foreach (string p in partitions)
            {
                Console.WriteLine(p);
            }

            Console.WriteLine(mindelay);
            Console.WriteLine(maxdelay);
            //Console.WriteLine("[ToString] end");
            string end = "[ToString] end";
            return end;
        }

    }

    class ServerServices : ServerService.ServerServiceBase
    {

        Dictionary<(string, string), string> dataStorage = new Dictionary<(string, string), string>();

        ServerInfo myinfo;

        List<ServerInfo> serversinfo = new List<ServerInfo>();

        public ServerServices(string[] args) {

            Console.WriteLine("[SERVER_SERVICE] Entrou no construtor");
            Console.WriteLine("[SERVER_SERVICE] [myinfo] start");
            myinfo = new ServerInfo(args[0]);
            Console.WriteLine(myinfo.ToString());
   
            Console.WriteLine("[SERVER_SERVICE] [myinfo] end");

            Console.WriteLine("[SERVER_SERVICE] [servers info] start");

       
            for (int i = 1;  i < args.Length; i++)
            {
                serversinfo.Add( new ServerInfo(args[i]));
            }

            foreach( ServerInfo s in serversinfo)
            {
                Console.WriteLine(s.ToString());
            }
            
            Console.WriteLine("[SERVER_SERVICE] [servers info] end");
        }



        public override Task<ReadReply> Read(ReadRequest request, ServerCallContext context)
        {
            Thread.Sleep((new Random()).Next(myinfo.Mindelay, myinfo.Maxdelay));
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

            Thread.Sleep((new Random()).Next(myinfo.Mindelay, myinfo.Maxdelay));
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
            //id url partitionsMaster(-1) partitions mindelay maxdelay; id ...

            /*ArrayList servers = new ArrayList();

            for (int i = 0; i < args.Length; i++) { servers.Add(args[i]); }
            
                      
            string id = args[0];
            Uri uri = new Uri(args[1]);
            int mindelay = Int32.Parse(args[2]);
            int maxdelay = Int32.Parse(args[3]);*/

            Uri uri = new Uri(args[0].Split("|")[1]);
            

            Server server = new Server
            {
                Services = { ServerService.BindService(new ServerServices(args)) },
                Ports = { new ServerPort(uri.Host, uri.Port, ServerCredentials.Insecure) }
            };

            server.Start();
            Console.ReadKey();
            server.ShutdownAsync().Wait();

        }
        
    }
}
