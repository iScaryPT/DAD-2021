using System;
using System.Collections.Generic;
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
            for (int i = 3; i < args.Length; i++)
            {
                serversi.Add(new ServerInfo(args[i]));
            }

        }
        public void Connect(string host)
        {

            if (this.channel != null)
                this.shutDown();

            AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            channel = GrpcChannel.ForAddress(host);
            client = new ServerService.ServerServiceClient(channel);
            Console.WriteLine($"Changing server to {host} ....");
            serverUrl = host;
        }
        public string Read(string partitionId, string objectId, string serverId) {
            bool alternWorked = false;
            if (this.client == null)
            {
                int idx = (new Random()).Next(0, serversi.Count);
                this.Connect(serversi[idx].Url);                
            }

            string response = "N/A";
            int serverIdx = serversi.FindIndex(serv => serv.Url.Equals(this.serverUrl));

            if (serversi[serverIdx].IsAvailable)
            {
                try
                {
                    ReadReply reply = client.Read(new ReadRequest
                    {
                        ObjectId = objectId,
                        PartitionId = partitionId
                    });

                    response = reply.ObjectValue;

                }
                catch (Exception)
                {
                    Console.WriteLine($"Server {this.serverUrl} not available.");

                    this.channel = null;
                    this.client = null;
                    int failedIdx = serversi.FindIndex(failed => failed.Url.Equals(this.serverUrl));
                    serversi[failedIdx].IsAvailable = false;
                    this.serverUrl = "";

                }
            }
            
            if (response.Equals("N/A") && !serverId.Equals("-1"))
            {
                string url = findServerbyId(serverId);
                while (!alternWorked)
                {
                    serverIdx = serversi.FindIndex(serv => serv.Url.Equals(url));
                    if (serversi[serverIdx].IsAvailable)
                    {
                        if (!serverUrl.Equals(url))
                        {
                            this.Connect(url);
                        }

                        try
                        {
                            response = client.Read(new ReadRequest
                            {
                                ObjectId = objectId,
                                PartitionId = partitionId
                            }).ObjectValue;
                            break;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"Server {this.serverUrl} not available.");
                            this.channel = null;
                            this.client = null;
                            int failedIdx = serversi.FindIndex(failed => failed.Url.Equals(this.serverUrl));
                            serversi[failedIdx].IsAvailable = false;
                            url = findAvailableServerByPartition(partitionId);
                            this.serverUrl = "";
                            if (url == null)
                            {
                                break;
                            }
                        }
                    }
                    else { 
                        url = findAvailableServerByPartition(partitionId);
                        this.serverUrl = "";
                        if (url == null)
                        {
                            break;
                        }
                    }
                }
            }

            //ignore my broken pipe it still works
            return response;
        }
        public bool Write(string partitionId, string objectId, string value) {
            int maxTries = 3;
            WriteReply reply = new WriteReply();
            string tmpUrl = findMasterbyPartition(partitionId);
            if (this.client == null || !serverUrl.Equals(tmpUrl))
            {  
                this.Connect(tmpUrl);
            }
            while (maxTries != 0)
            {
                try
                {
                    reply = client.Write(new WriteRequest
                    {

                        PartitionId = partitionId,
                        ObjectId = objectId,
                        ObjectValue = value

                    });
                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Server {this.serverUrl} not available.");
                    this.channel = null;
                    this.client = null;
                    reply.Ok = false;
                    maxTries--;
                    string newMaster = newPartitionMaster(partitionId, this.serverUrl);
                    this.Connect(newMaster);
                }
            }

            return reply.Ok;
        }
        public string listGlobal() {

            string res = "";
            foreach(ServerInfo s in serversi)
            {
                res += "[Server " + s.Name + "]\r\n";
                res += this.listServer(s.Name) + "\r\n";
            }

            return res;
        }
        public string listServer(string serverId) 
        {
            ListServerReply reply = new ListServerReply { };
            string tmpUrl = findServerbyId(serverId);
            if (this.client == null || !serverUrl.Equals(tmpUrl))
            {
                this.Connect(tmpUrl);
            }
            int serverIdx = serversi.FindIndex(serv => serv.Url.Equals(tmpUrl));
            if (serversi[serverIdx].IsAvailable)
            {
                try
                {
                    reply = client.ListServer(new ListServerRequest { });
                }
                catch (Exception)
                {
                    reply.Objects = "Not available\r\n";
                }
            } else { reply.Objects = "Not available\r\n"; }
            return reply.Objects;

        }
        public void shutDown()
        {
            channel.ShutdownAsync().Wait();
        }
        public string newPartitionMaster(string partitionId, string oldMaster)
        {

            int randomServerIdx = (new Random()).Next(0, serversi.Count - 1);
            string informantUrl = serversi[randomServerIdx].Url;
            this.Connect(informantUrl);
            NewPartitionMasterReply reply = this.client.GiveNewPartitionMaster(new NewPartitionMasterRequest { PartitionId = partitionId });
            
            foreach(ServerInfo serverInfo in serversi)
            {
                if (serverInfo.Url.Equals(oldMaster))
                {
                    serversi.Remove(serverInfo);
                    break;
                }
            }

            foreach(ServerInfo serverInfo in serversi)
            {
                if (serverInfo.Url.Equals(reply.NewMaster))
                {
                    serverInfo.Master.Add(partitionId);
                    if (!serverInfo.Partitions.Contains(partitionId))
                    {
                        serverInfo.Partitions.Add(partitionId);
                    }
                    break;
                }
            }

            return reply.NewMaster;
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

        public string findAvailableServerByPartition(string partitionId)
        {
            foreach(ServerInfo server in serversi)
            {
                if(server.Partitions.Contains(partitionId) && server.IsAvailable)
                {
                    return server.Url;
                }
            }

            return null;
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
        private bool isAvailable;
        public string Name { get { return name; } }
        public string Url { get { return url; } }
        public List<string> Master { get { return master; } }
        public List<string> Partitions { get { return partitions; } }
        public int Mindelay { get { return mindelay; } }
        public int Maxdelay { get { return maxdelay; } }
        public bool IsAvailable { get { return isAvailable; } set { this.isAvailable = value; } }

        public ServerInfo(string info)
        {
            string[] ser = info.Split("|");
            this.name = ser[0];
            this.url = ser[1];
            this.master = new List<string>(ser[2].Split(","));
            this.partitions = new List<string>(ser[3].Split(","));
            this.mindelay = Int32.Parse(ser[4]);
            this.maxdelay = Int32.Parse(ser[5]);
            this.isAvailable = true;
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
            string end = "[ToString] end";
            return end;
        }
    }
}
