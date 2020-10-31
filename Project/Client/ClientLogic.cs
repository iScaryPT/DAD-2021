using System;
using System.Collections.Generic;
using System.Text;
using ConfigStorageSP;
using Grpc.Net.Client;

namespace ClientLogicSP
{

    class ClientLogic
    {
        private readonly ConfigStorage config;
        private GrpcChannel channel;
        private ServerService.ServerServiceClient client;
        private string serverUrl;

        public ClientLogic(string host, ConfigStorage config) {

            this.config = config;
            this.serverUrl = host;
            AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            this.channel = GrpcChannel.ForAddress(host);
            client = new ServerService.ServerServiceClient(channel);
        }

        public string Read(int partitionId, int objectId, int serverId) {

            ReadReply reply = client.Read(new ReadRequest
            {
                ObjectId = objectId,
                PartitionId = partitionId
            });

            if (reply.ObjectValue.ToString().Equals("N/A"))
            {
                if(serverId == -1)
                {   
                    string newServerUrl = config.findRandomServerByPartition(partitionId);
                    while(newServerUrl.Equals(this.serverUrl))
                        newServerUrl = config.findRandomServerByPartition(partitionId);
                    this.changeServer(newServerUrl);

                } else
                {
                    string newServerUrl = config.findServerById(serverId);
                    if (!newServerUrl.Equals(this.serverUrl))
                    {
                        this.changeServer(newServerUrl);
                    }

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
        public bool Write(int partitionId, int objectId, string value) {

            string masterServer = config.findMServerByPartition(partitionId);
            if (!this.serverUrl.Equals(masterServer))
            {
                this.changeServer(masterServer);
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
            this.channel.ShutdownAsync();
        }

        public void changeServer(string host)
        {
            this.shutDown();
            this.serverUrl = host;
            this.channel = GrpcChannel.ForAddress(this.serverUrl);
            this.client = new ServerService.ServerServiceClient(this.channel);
        }

    }
}
