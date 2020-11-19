using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;
using Google.Protobuf.Reflection;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Newtonsoft.Json.Linq;

namespace ServerSP
{
    /*
    class ServerInterceptor : Interceptor
    {
        private bool is_intercepting;

        public ServerInterceptor()
        {
            this.is_intercepting = false;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            while (!is_intercepting)
                await Task.Delay(25);

            var response = await base.UnaryServerHandler(request, context, continuation);
            return response;
        }

        public void intercept()
        {
            this.is_intercepting = !this.is_intercepting;
        }
    }
    */

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
            this.master = new List<string>(ser[2].Split(","));
            this.partitions = new List<string>(ser[3].Split(","));
            this.mindelay = Int32.Parse(ser[4]);
            this.maxdelay = Int32.Parse(ser[5]);
        }


    }

    class ServerServices : ServerService.ServerServiceBase
    {

        Dictionary<(string, string), string> dataStorage = new Dictionary<(string, string), string>();

        ServerInfo myinfo;

        List<ServerInfo> serversinfo = new List<ServerInfo>();

        //works for stablish a connection to all servers
        private GrpcChannel channel;
        private ServerService.ServerServiceClient server;
        //private ServerInterceptor interceptor;

        private bool isFreezing;
        private object fLock;

        private Server s;

        public void BroadCastMessage(string host, string partitionId, string objectId, string value)
        {
            if (this.channel != null)
                this.shutDown();
            AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            channel = GrpcChannel.ForAddress(host);
            server = new ServerService.ServerServiceClient(channel);
            Console.WriteLine($"Broadcasting write to {host} ...");

            server.BroadcastMsg(new BroadcastMessageRequest { 
                PartitionId = partitionId,
                ObjectId = objectId,
                Message = value
            });
        }

        public ServerServices(string[] args, Server server) {

            myinfo = new ServerInfo(args[0]);

            for (int i = 1;  i < args.Length; i++)
            {
                serversinfo.Add( new ServerInfo(args[i]));
            }

            //this.interceptor = interceptor;
            this.isFreezing = false;
            this.fLock = new object();
            this.s = server;
        }

        public void shutDown()
        {
            channel.ShutdownAsync().Wait();
        }


        public override Task<CrashReply> Crash(CrashRequest request, ServerCallContext context)
        {
            this.s.ShutdownAsync().Wait();
            return Task.FromResult(new CrashReply { });
        }

        public override Task<StatusReply> Status( StatusRequest request, ServerCallContext context)
        {
            Console.WriteLine("-------[Debug]-------");

            string mp = "";
            foreach (string s in this.myinfo.Master)
                mp += s + " ";

            string sp = "";
            foreach (string partition in this.myinfo.Partitions)
                sp += partition + " ";

            Console.WriteLine($"Server Master of: {mp}");
            Console.WriteLine($"Replicated Partitions: {sp}");
            Console.WriteLine("Freeze: " + (this.isFreezing ? "YES" : "NO"));
            Console.WriteLine("---------------------");

            return Task.FromResult(new StatusReply { });
        }

        public override Task<BroadcastMessageReply> BroadcastMsg(BroadcastMessageRequest request, ServerCallContext context)
        {
            this.tryFreeze();
            Thread.Sleep((new Random()).Next(myinfo.Mindelay, myinfo.Maxdelay));

            lock (this)
            {
                dataStorage[(request.PartitionId, request.ObjectId)] = request.Message;
            }

            return Task.FromResult(new BroadcastMessageReply { });
        }

        public override Task<ReadReply> Read(ReadRequest request, ServerCallContext context)
        {
            this.tryFreeze();
            Console.WriteLine("Executing read command...");

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
            this.tryFreeze();
            Console.WriteLine("Executing write command...");
            Thread.Sleep((new Random()).Next(myinfo.Mindelay, myinfo.Maxdelay));

            List<string> allUrls = this.findServersByPartition(request.PartitionId);

            lock (this)
            {
                dataStorage[(request.PartitionId, request.ObjectId)] = request.ObjectValue;
                foreach (string host in allUrls)
                    this.BroadCastMessage(host, request.PartitionId, request.ObjectId, request.ObjectValue);
            }
                        
            return Task.FromResult(new WriteReply { Ok = true });
        }

        public override Task<ListServerReply> ListServer(ListServerRequest request, ServerCallContext context)
        {
            this.tryFreeze();
            Console.WriteLine("Executing list server command...");

            Thread.Sleep((new Random()).Next(myinfo.Mindelay, myinfo.Maxdelay));

            string res = "";
            List<string> master = myinfo.Master;

            lock (this)
            {
                foreach(KeyValuePair<(string, string),string> pair in dataStorage){
                    res += "<" + pair.Key.Item1 + "," + pair.Key.Item2 + "> : " + pair.Value;
                    res += (master.Contains(pair.Key.Item1)) ? " - MASTER\r\n" : "\r\n";
                }
            }

            return Task.FromResult(new ListServerReply { Objects = res });
        }
        
        public override Task<FreezeReply> Freeze(FreezeRequest request, ServerCallContext context)
        {
            this.isFreezing = true;
            return Task.FromResult(new FreezeReply { });
        }

        public override Task<UnFreezeReply> UnFreeze(UnFreezeRequest request, ServerCallContext context)
        {
            this.unFreeze();
            return Task.FromResult(new UnFreezeReply { });
        }
        
        public List<string> findServersByPartition(string partitionId)
        {
            List<string> res = new List<string>();

            foreach(ServerInfo s in serversinfo)
            {
                if (s.Partitions.Contains(partitionId))
                    res.Add(s.Url);
            }
            return res;
        }

        public void tryFreeze()
        {
            lock (this.fLock)
            {
                if (isFreezing)
                    Monitor.Wait(this.fLock);
            }
        }

        public void unFreeze()
        {
            lock (this.fLock)
            {
                if (isFreezing)
                    Monitor.PulseAll(this.fLock);
                this.isFreezing = false;
            }
        }

    }

    class Program
    {

        public static void Main(string[] args)
        {

            Uri uri = new Uri(args[0].Split("|")[1]);

            //ServerInterceptor interceptor = new ServerInterceptor();
            /*
            Server server = new Server
            {
                Services = { ServerService.BindService(new ServerServices(args)) },
                Ports = { new ServerPort(uri.Host, uri.Port, ServerCredentials.Insecure) }
            };
            */
            Server server = new Server();
            server.Services.Add(ServerService.BindService(new ServerServices(args, server)));
            server.Ports.Add(new ServerPort(uri.Host, uri.Port, ServerCredentials.Insecure));

            server.Start();
            Console.WriteLine($"Server listening on host {uri.Host} and port {uri.Port}");
            Console.ReadKey();
            server.ShutdownAsync().Wait();

        }
        
    }
}
