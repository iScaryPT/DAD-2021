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
            Console.WriteLine($"Hi! My name is {name} ...");
            this.url = ser[1];
            List<string> masterP = new List<string>(ser[2].Split(","));
            if (masterP.Contains("null"))
            {
                this.master = new List<string>();
            }
            else
            {
                this.master = new List<string>(masterP);
                this.master.Remove("");
                foreach(string mp in master)
                {
                    Console.WriteLine($"I am master of {mp} ...");
                }
            }
            this.partitions = new List<string>(ser[3].Split(","));
            this.partitions.Remove("");
            foreach(string p in partitions)
            {
                Console.WriteLine($"I belong to {p} ...");
            }
            this.mindelay = Int32.Parse(ser[4]);
            this.maxdelay = Int32.Parse(ser[5]);
        }
    }

    class ServerObject
    {
        string objValue;
        int seqNum;

        public ServerObject(string newVal, int newSeqNum)
        {
            this.objValue = newVal;
            this.seqNum = newSeqNum;
        }

        public string ObjValue { 
            get { return this.objValue; } 
            set { this.objValue = value; } 
        }

        public int SeqNum
        {
            get { return this.seqNum; }
            set { this.seqNum = value; }
        }
    }

    class ServerServices : ServerService.ServerServiceBase
    {

        Dictionary<(string, string), ServerObject> dataStorage = new Dictionary<(string, string), ServerObject>();
        //Dictionary<(string, string), int> objectSequenceNum = new Dictionary<(string, string), int>();

        ServerInfo myinfo;

        List<ServerInfo> serversinfo = new List<ServerInfo>();

        //works for establish a connection to all servers
        private GrpcChannel channel;
        private ServerService.ServerServiceClient server;

        private bool isFreezing;
        private object fLock;
        private string guardianUrl;
        private string vipUrl;
        private object vipLock;

        public void BroadCastMessage(string host, string partitionId, string objectId, string value, int objSeqNum)
        {
            if (this.channel != null)
                this.shutDown();
            AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            channel = GrpcChannel.ForAddress(host);
            server = new ServerService.ServerServiceClient(channel);
            try
            {
                server.BroadcastMsg(new BroadcastMessageRequest
                {
                    PartitionId = partitionId,
                    ObjectId = objectId,
                    Message = value,
                    ObjectSeqNum = objSeqNum
                });
                Console.WriteLine($"Broadcasting write to {host} ...");
            } catch (Exception)
            {
                Console.WriteLine($"Broadcast to server {host} failed...");
            }
            
        }

        public ServerServices(string[] args) {

            myinfo = new ServerInfo(args[0]);

            for (int i = 1;  i < args.Length; i++)
            {
                serversinfo.Add( new ServerInfo(args[i]));
            }

            this.isFreezing = false;
            this.fLock = new object();
            this.vipLock = new object();
            this.vipUrl = "";
            this.guardianUrl = "";

            if (!(myinfo.Master.Count == 0))
            {
                Task.Run(() => guardianSetup());
            }
        }

        public void shutDown()
        {
            channel.ShutdownAsync().Wait();
        }

        public void guardianSetup()
        {
            Console.WriteLine($"<New Task> Will now proceed to setup with a guardian...");
            while (chooseGuardian())
            {
                pingGuardian();
            }
        }

        public bool chooseGuardian()
        {
            Thread.Sleep(2000);

            GrpcChannel channel;
            ServerService.ServerServiceClient server;
            GuardianReply reply = new GuardianReply { Ok = false };
            int guardianIdx = 0;
            List<ServerInfo> serversInfo = new List<ServerInfo>(this.serversinfo);

            while (reply.Ok == false)
            {
                if(serversInfo.Count == 0)
                {
                    return false;
                }
                guardianIdx = (new Random()).Next(0, 1000) % serversInfo.Count;
                AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                channel = GrpcChannel.ForAddress(serversInfo[guardianIdx].Url);
                server = new ServerService.ServerServiceClient(channel);
                //Console.WriteLine($"Will try to make {serversInfo[guardianIdx].Url} my new guardian...");
                try
                {
                    reply = server.guardianRequest(new GuardianRequest { Url = myinfo.Url});
                }
                catch (Exception)
                {
                    Console.WriteLine($"Can't connect to guardian {serversInfo[guardianIdx].Url}");
                    serversInfo.Remove(serversInfo[guardianIdx]);
                    channel.ShutdownAsync().Wait();
                }
            }
            this.guardianUrl = serversInfo[guardianIdx].Url;
            return true;
        }

        public void pingGuardian()
        {
            GrpcChannel channel;
            ServerService.ServerServiceClient server;

            AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            channel = GrpcChannel.ForAddress(this.guardianUrl);
            server = new ServerService.ServerServiceClient(channel);
            Console.WriteLine($"Pinging to guardian {this.guardianUrl} ...");

            while (true)
            {
                Thread.Sleep(500);
                try
                {
                    PingReply reply = server.Ping(new PingRequest { });
                }
                catch (Exception)
                {
                    //Guardian might have died
                    channel.ShutdownAsync().Wait();
                    return;
                }
            }
        }

        public void pingVip()
        {
            GrpcChannel channel;
            ServerService.ServerServiceClient server;

            AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            channel = GrpcChannel.ForAddress(this.vipUrl);
            server = new ServerService.ServerServiceClient(channel);
            Console.WriteLine($"Pinging to VIP {this.vipUrl} ...");

            while (true)
            {
                Thread.Sleep(500);
                try
                {
                    PingReply reply = server.Ping(new PingRequest { });
                }
                catch (Exception)
                {
                    //Master might have died
                    //TODO ELECTION DAY
                    masterElection();
                    lock (vipLock)
                    {
                        this.vipUrl = "";
                    }
                    channel.ShutdownAsync().Wait();
                    return;
                }
            }
        }

        public void masterElection()
        {
            ServerInfo newMaster;
            GrpcChannel channel;
            ServerService.ServerServiceClient server;
            AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            List<ServerObjectInfo> updatedDataStorage = new List<ServerObjectInfo>();
            ServerInfo deadMaster = getServerByUrl(this.vipUrl);
            string newMasterUrl = deadMaster.Url;
            int newMasterIdx = 0;
            int deadMasterPartitions = deadMaster.Master.Count;
            Console.WriteLine($"The dead master {deadMaster.Url} was master of {deadMasterPartitions} partitions...");

            while (newMasterUrl.Equals(deadMaster.Url))
            {
                newMasterIdx = (new Random()).Next(0, 1000) % serversinfo.Count;
                newMasterUrl = serversinfo[newMasterIdx].Url;
            }

            newMaster = serversinfo[newMasterIdx];
            Console.WriteLine($"{newMasterUrl} is the new master...");

            for(int i = 0; i < deadMasterPartitions; i++)
            {
                Console.WriteLine($"Proceeding to get updates from {deadMaster.Master[i]} ...");
                Dictionary<(string, string), ServerObject> updatedPartitionObjs = getUpdatedPartitionObjects(deadMaster.Master[i]);
                
                foreach(KeyValuePair<(string, string), ServerObject> pair in updatedPartitionObjs)
                {
                    updatedDataStorage.Add(new ServerObjectInfo
                    {
                        PartitionId = pair.Key.Item1,
                        ObjectId = pair.Key.Item2,
                        ObjectValue = pair.Value.ObjValue,
                        ObjectSeqNum = pair.Value.SeqNum
                    });
                }
            }
            //Send updated dictionary to new master
            channel = GrpcChannel.ForAddress(newMasterUrl);
            server = new ServerService.ServerServiceClient(channel);
            //TODO: ERROR HANDLING
            Console.WriteLine($"Giving the new master {newMasterUrl} the updates...");
            server.UpdateMaster(new UpdateMasteRequest { MasterPartitions = { deadMaster.Master}, 
                ObjectInfo = { updatedDataStorage }, DeadMasterId = deadMaster.Name });
        }

        public override Task<UpdateMasterReply> UpdateMaster(UpdateMasteRequest request, ServerCallContext context)
        {
            GrpcChannel channel;
            ServerService.ServerServiceClient server;
            AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            Console.WriteLine($"I {myinfo.Url} am now updating myself");
            foreach (string partition in request.MasterPartitions)
            {
                Console.WriteLine($"I am the new master of partition {partition}");
                myinfo.Master.Add(partition);
                if (!myinfo.Partitions.Contains(partition))
                {
                    myinfo.Partitions.Add(partition);
                }
            }
            //TODO Lock storage
            foreach(ServerObjectInfo objInfo in request.ObjectInfo)
            {
                if(dataStorage.ContainsKey((objInfo.PartitionId, objInfo.ObjectId)))
                {
                    dataStorage[(objInfo.PartitionId, objInfo.ObjectId)].ObjValue = objInfo.ObjectValue;
                    dataStorage[(objInfo.PartitionId, objInfo.ObjectId)].SeqNum = objInfo.ObjectSeqNum;
                }

                else
                {
                    dataStorage[(objInfo.PartitionId, objInfo.ObjectId)] = new ServerObject(objInfo.ObjectValue, objInfo.ObjectSeqNum);
                }
            }
            //LOCK HERE?
            if (this.guardianUrl.Equals(""))
            {
                Console.WriteLine("It's my first time as a master! I need a guardian now...");
                Task.Run(() => guardianSetup());
            }

            foreach(ServerInfo serverInfo in serversinfo)
            {
                channel = GrpcChannel.ForAddress(serverInfo.Url);
                server = new ServerService.ServerServiceClient(channel);
                Console.WriteLine($"Proceeding to announce myself to {serverInfo.Url} ...");
                try
                {
                    server.AnnounceNewMaster(new AnnounceRequest { DeadMasterId = request.DeadMasterId, 
                        NewMasterId = myinfo.Name, NewMasterPartitions = { request.MasterPartitions } });
                    channel.ShutdownAsync().Wait();
                } catch(Exception)
                {
                    Console.WriteLine($"Couldn't announce that i'm new master to {serverInfo.Url} ...");
                    channel.ShutdownAsync().Wait();
                }
            }

            //broadcasting to all replicas (new method)
            broadcastObjsToReplicas(new List<ServerObjectInfo>(request.ObjectInfo), new List<string>(request.MasterPartitions));

            return Task.FromResult(new UpdateMasterReply { });
        }

        public void broadcastObjsToReplicas(List<ServerObjectInfo> objsInfo, List<string> masterPartitions)
        {
            Console.WriteLine("Broadcasting the updates to my homies...");
            foreach (string partition in masterPartitions)
            {
                Console.WriteLine($"Time to update partition {partition} ...");
                List<string> replicaUrls = findServersByPartition(partition);
                foreach(string url in replicaUrls)
                {
                    Console.WriteLine($"{url} is going to receive the new updates...");
                    foreach (ServerObjectInfo info in objsInfo) {
                        if (info.PartitionId.Equals(partition)) {
                            BroadCastMessage(url, info.PartitionId, info.ObjectId, info.ObjectValue, info.ObjectSeqNum);
                        }
                    }
                }
            }
        }

        public override Task<AnnounceReply> AnnounceNewMaster(AnnounceRequest request, ServerCallContext context)
        {
            //UPDATING NEW MASTER
            for(int i = 0; i < serversinfo.Count; i++)
            {
                if (serversinfo[i].Name.Equals(request.NewMasterId))
                {
                    foreach(string partition in request.NewMasterPartitions)
                    {
                        serversinfo[i].Master.Add(partition);
                        if (!serversinfo[i].Partitions.Contains(partition))
                        {
                            serversinfo[i].Partitions.Add(partition);
                        }
                    }
                }
            }

            //ERASING DEAD MASTER
            List<ServerInfo> serversInfo = new List<ServerInfo>(serversinfo);
            foreach(ServerInfo server in serversInfo)
            {
                if (server.Name.Equals(request.DeadMasterId))
                {
                    serversinfo.Remove(server);
                    break;
                }
            }

            return Task.FromResult(new AnnounceReply { });
        }

        public override Task<GuardianReply> guardianRequest(GuardianRequest request, ServerCallContext context)
        {
            lock (vipLock)
            {
                if (!this.vipUrl.Equals(""))
                {
                    return Task.FromResult(new GuardianReply { Ok = false });
                }
                this.vipUrl = request.Url;
            }
            //In case it isn't a guardian yet
            Task.Run(() => pingVip());
            return Task.FromResult(new GuardianReply { Ok = true });
        }

        public override Task<PingReply> Ping(PingRequest request, ServerCallContext context)
        {
            return Task.FromResult(new PingReply { });
        }

        public override Task<StatusReply> Status( StatusRequest request, ServerCallContext context)
        {
            Console.WriteLine("-------[Debug]-------");

            string mp = "";
            if (myinfo.Master.Count == 0)
            {
                mp = "None";
            }
            else
            {
                foreach (string s in this.myinfo.Master)
                    mp += s + " ";
            }

            string sp = "";
            foreach (string partition in this.myinfo.Partitions)
                sp += partition + " ";

            Console.WriteLine($"Server Master of: {mp}");
            Console.WriteLine($"Replicated Partitions: {sp}");
            Console.WriteLine("Freeze: " + (this.isFreezing ? "YES" : "NO"));
            Console.WriteLine("Pinging to VIP: " + (this.vipUrl.Equals("") ? "None" : this.vipUrl));
            Console.WriteLine("Pinging to Guardian : " + (this.guardianUrl.Equals("") ? "None" : this.guardianUrl));
            Console.WriteLine("---------------------");

            return Task.FromResult(new StatusReply { });
        }

        public override Task<BroadcastMessageReply> BroadcastMsg(BroadcastMessageRequest request, ServerCallContext context)
        {
            this.tryFreeze();
            Thread.Sleep((new Random()).Next(myinfo.Mindelay, myinfo.Maxdelay));

            lock (this)
            {
                //dataStorage[(request.PartitionId, request.ObjectId)] = request.Message;
                //objectSequenceNum[(request.PartitionId, request.ObjectId)] = request.ObjectSeqNum;
                dataStorage[(request.PartitionId, request.ObjectId)] = new ServerObject(request.Message, request.ObjectSeqNum);
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
                    res = dataStorage[(request.PartitionId, request.ObjectId)].ObjValue;
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
                if (!dataStorage.ContainsKey((request.PartitionId, request.ObjectId)))
                {
                    //objectSequenceNum[(request.PartitionId, request.ObjectId)] = 1;
                    dataStorage[(request.PartitionId, request.ObjectId)] = new ServerObject(request.ObjectValue, 1);
                }
                else
                {
                    //objectSequenceNum[(request.PartitionId, request.ObjectId)]++;

                    dataStorage[(request.PartitionId, request.ObjectId)].ObjValue = request.ObjectValue;
                    dataStorage[(request.PartitionId, request.ObjectId)].SeqNum += 1;
                }
                //dataStorage[(request.PartitionId, request.ObjectId)] = request.ObjectValue;
                
                foreach (string host in allUrls)
                    this.BroadCastMessage(host, request.PartitionId, request.ObjectId,
                        dataStorage[(request.PartitionId, request.ObjectId)].ObjValue, 
                        dataStorage[(request.PartitionId, request.ObjectId)].SeqNum);
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
                foreach(KeyValuePair<(string, string),ServerObject> pair in dataStorage){
                    res += "<" + pair.Key.Item1 + "," + pair.Key.Item2 + "> : " + pair.Value.ObjValue 
                        + " - SeqNum: " + pair.Value.SeqNum;
                    res += (master.Contains(pair.Key.Item1)) ? " - MASTER\r\n" : "\r\n";
                }
            }

            return Task.FromResult(new ListServerReply { Objects = res });
        }
        public override Task<NewPartitionMasterReply> GiveNewPartitionMaster(NewPartitionMasterRequest request, ServerCallContext context)
        {
            foreach(ServerInfo serverInfo in serversinfo)
            {
                if (serverInfo.Master.Contains(request.PartitionId))
                {
                    return Task.FromResult(new NewPartitionMasterReply { NewMaster = serverInfo.Url });
                }
            }

            return Task.FromResult(new NewPartitionMasterReply { });
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

        public Dictionary<(string,string), ServerObject> getUpdatedPartitionObjects(string partitionId)
        {
            GrpcChannel channel;
            ServerService.ServerServiceClient server;
            Dictionary<(string, string), ServerObject> updatedPartition = new Dictionary<(string, string), ServerObject>();
            AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            Console.WriteLine($"Getting the updated partition objects of partition {partitionId} ...");

            List<string> partitionMembers = findServersByPartition(partitionId);

            if (myinfo.Partitions.Contains(partitionId))
            {
                partitionMembers.Add(myinfo.Url);
            }

            foreach(string url in partitionMembers)
            {
                Console.WriteLine($"It's {url} turn to give updates...");
               if(url.Equals(myinfo.Url))
               {
                   //CHECK LOCK
                   Console.WriteLine($"I {myinfo.Url} am a member of this partition...");
                   foreach(KeyValuePair<(string, string), ServerObject> pair in dataStorage)
                    {
                        if (pair.Key.Item1.Equals(partitionId))
                        {
                            if (updatedPartition.ContainsKey((pair.Key.Item1, pair.Key.Item2)))
                            {
                                if (updatedPartition[(pair.Key.Item1, pair.Key.Item2)].SeqNum < pair.Value.SeqNum)
                                {
                                    updatedPartition[(pair.Key.Item1, pair.Key.Item2)].ObjValue = pair.Value.ObjValue;
                                    updatedPartition[(pair.Key.Item1, pair.Key.Item2)].SeqNum = pair.Value.SeqNum;
                                }
                            }

                            else
                            {
                                updatedPartition[(pair.Key.Item1, pair.Key.Item2)] = new ServerObject(pair.Value.ObjValue, pair.Value.SeqNum);
                            }
                        }
                    }
               }

               else
               {
                    channel = GrpcChannel.ForAddress(url);
                    server = new ServerService.ServerServiceClient(channel);
                    try
                    {
                        Console.WriteLine($"Will try to connect to {url} to get updates of {partitionId}...");
                        PartitionObjectsReply reply = server.GetPartitionObjects(new PartitionObjectsRequest { PartitionId = partitionId });
                        foreach(ServerObjectInfo objInfo in reply.ObjectInfo)
                        {
                            if(updatedPartition.ContainsKey((objInfo.PartitionId, objInfo.ObjectId)))
                            {
                                if(updatedPartition[(objInfo.PartitionId, objInfo.ObjectId)].SeqNum < objInfo.ObjectSeqNum)
                                {
                                    updatedPartition[(objInfo.PartitionId, objInfo.ObjectId)].ObjValue = objInfo.ObjectValue;
                                    updatedPartition[(objInfo.PartitionId, objInfo.ObjectId)].SeqNum = objInfo.ObjectSeqNum;
                                }
                            }

                            else
                            {
                                updatedPartition[(objInfo.PartitionId, objInfo.ObjectId)] = new ServerObject(objInfo.ObjectValue, objInfo.ObjectSeqNum);
                            }
                        }
                        channel.ShutdownAsync().Wait();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Couldn't get the partition's {partitionId} update from {url} ...");
                        channel.ShutdownAsync().Wait();
                    }
               }
            }
            Console.WriteLine($"Got all updates from {partitionId}! Sending it to new master...");
            return updatedPartition;
        }

        public override Task<PartitionObjectsReply> GetPartitionObjects(PartitionObjectsRequest request, ServerCallContext context)
        {
            //CHECK LOCK
            List<ServerObjectInfo> objsInfo = new List<ServerObjectInfo>();
            foreach(KeyValuePair<(string, string), ServerObject> pair in dataStorage)
            {
                if (pair.Key.Item1.Equals(request.PartitionId))
                {
                    objsInfo.Add(new ServerObjectInfo { PartitionId = pair.Key.Item1, ObjectId = pair.Key.Item2, 
                        ObjectValue = pair.Value.ObjValue, ObjectSeqNum = pair.Value.SeqNum });
                }
            }

            return Task.FromResult(new PartitionObjectsReply { ObjectInfo = { objsInfo } });
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

        public ServerInfo getServerByUrl(string url)
        {
            foreach(ServerInfo server in serversinfo)
            {
                if (server.Url.Equals(url))
                {
                    return server;
                }
            }

            return null;
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
            Server server = new Server
            {
                Services = { ServerService.BindService(new ServerServices(args)) },
                Ports = { new ServerPort(uri.Host, uri.Port, ServerCredentials.Insecure) }
            };

            server.Start();
            Console.WriteLine($"Server listening on host {uri.Host} and port {uri.Port}");
            Console.ReadKey();
            server.ShutdownAsync().Wait();

        }
        
    }
}
