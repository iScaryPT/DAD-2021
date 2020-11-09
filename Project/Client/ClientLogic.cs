using System;
using System.Collections.Generic;
using System.Text;
using ClientSP;
using ConfigStorageSP;
using Grpc.Net.Client;

namespace ClientLogicSP
{

    class ClientLogic
    {
        private GrpcChannel channel;
        private ServerService.ServerServiceClient client;
        private string serverUrl;

        List<ServerInfo> serversi = new List<ServerInfo>();
        public ClientLogic(string[]args) {
            for (int i = 0; i < args.Length; i++)
            {
                serversi.Add(new ServerInfo(args[i]));
            }

        }

        public void Connect(string host)
        {
            serverUrl = host;
            AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            channel = GrpcChannel.ForAddress(host);
            client = new ServerService.ServerServiceClient(channel);

        }

        public string Read(string partitionId, string objectId, string serverId) {

            if (this.client == null)
            {
                int idx = (new Random()).Next(0, serversi.Count);
                this.serverUrl = serversi[idx].Url;
                this.Connect(serverUrl);                
            }
            


            ReadReply reply = client.Read(new ReadRequest
            {
                ObjectId = objectId,
                PartitionId = partitionId
            });

            if (reply.ObjectValue.Equals("N/A") && !serverId.Equals("-1"))
            {
                string url = findServerbyId(serverId);
                if (!serverUrl.Equals(url))
                {
                    this.Connect(serverUrl);

                }
                reply = client.Read(new ReadRequest
                {
                    ObjectId = objectId,
                    PartitionId = partitionId
                });
            }

            //ignore my broken pipe it still works
            return reply.ObjectValue.ToString();
        }
        public bool Write(string partitionId, string objectId, string value) {

            if (this.client == null || serverUrl != findMasterbyPartition(partitionId))
            {
                this.serverUrl = findMasterbyPartition(partitionId);
                this.Connect(serverUrl);
            }


            WriteReply reply = client.Write(new WriteRequest { 
                
                PartitionId = partitionId,
                ObjectId = objectId,
                ObjectValue = value

            });

            return reply.Ok;
        }
        public void listGlobal() { }
        public void listServer() { }
        public void shutDown()
        {
            channel.ShutdownAsync();
        }

        public void changeServer(string host)
        {
            shutDown();
            serverUrl = host;
            channel = GrpcChannel.ForAddress(serverUrl);
            client = new ServerService.ServerServiceClient(channel);
        }

        public string findMasterbyPartition(string partitionId)
        {
            foreach(ServerInfo sinfo in serversi)
            {
                if(sinfo.Master.Contains(partitionId))
                {
                    return sinfo.Url;
                }
            }

            return "";
        }

        public string findServerbyId(string serverid)
        {
            foreach (ServerInfo sinfo in serversi)
            {
                if (sinfo.Name.Equals(serverid))
                {
                    return sinfo.Url;
                }
            }
            return "";
        }

    }

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
}
