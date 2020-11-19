using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using ClientSP;
using Grpc.Net.Client;

namespace ClientLogicSP
{

    class ClientLogic
    {
        private GrpcChannel channel;
        private ServerService.ServerServiceClient client;
        private string serverUrl;
        private int maxTries = 3;

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


            if (this.client == null)
            {
                int idx = (new Random()).Next(0, serversi.Count);
                this.Connect(serversi[idx].Url);                
            }

            string response = "N/A";
       
            try
            {
                ReadReply reply = client.Read(new ReadRequest
                {
                    ObjectId = objectId,
                    PartitionId = partitionId
                });

                response = reply.ObjectValue;

            } catch (Exception) 
            {
                Console.WriteLine($"Server {this.serverUrl} not available.");
                this.channel = null;
                this.client = null;
                removeServerUrlfromList(serverUrl);
                this.serverUrl = "";
               
            }
            

            if (response.Equals("N/A") && !serverId.Equals("-1"))
            {
                string url = findServerbyId(serverId);
 
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
                }
                catch (Exception) 
                {
                    Console.WriteLine($"Server {this.serverUrl} not available.");
                    this.channel =  null;
                    this.client = null;
                    removeServerUrlfromList(serverUrl);
                    this.serverUrl = "";
                    
                }
            }

            //ignore my broken pipe it still works
            return response;
        }
        public bool Write(string partitionId, string objectId, string value) {

            string tmpUrl = findMasterbyPartition(partitionId);
            if (this.client == null || !serverUrl.Equals(tmpUrl))
            {  
                this.Connect(tmpUrl);
            }

            try(){
                WriteReply reply = client.Write(new WriteRequest
                {

                    PartitionId = partitionId,
                    ObjectId = objectId,
                    ObjectValue = value

                });
            }catch (Exception)
            {
                Console.WriteLine($"Server {this.serverUrl} not available.");
                this.channel = null;
                this.client = null;
                this.serverUrl = "";

                removeServerIdfromList(tmpUrl);
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
            string tmpUrl = findServerbyId(serverId);
            if (this.client == null || !serverUrl.Equals(tmpUrl))
            {
                this.Connect(tmpUrl);
            }

            ListServerReply reply = client.ListServer(new ListServerRequest { });

            return reply.Objects;

        }
        public void shutDown()
        {
            channel.ShutdownAsync().Wait();
        }
        /*
        public void freeze(string server)
        {
            FreezeReply reply = client.Freeze(new FreezeRequest { });
        }
        */

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

    
        public void removeServerUrlfromList(string url)
        {
            string serverid = "";
            foreach (ServerInfo si in serversi){
                if (si.Url.Equals(url){
                    serverid = si.Name;
                    serversi.Remove(si);
                }
            }
            foreach (ServerInfo si in serversi)
            {
                if (si.Partitions.Contains(serverid))
                {
                    si.Partitions.Remove(serverid)
                }
            }
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
            this.master = new List<string>(ser[2].Split(","));
            this.partitions = new List<string>(ser[3].Split(","));
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
