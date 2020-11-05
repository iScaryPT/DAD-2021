using System;
using System.Collections.Generic;
using System.Text;
using ConfigStorageSP;
using Grpc.Net.Client;

namespace ClientLogicSP
{

    class ClientLogic
    {
        private GrpcChannel channel;
        private ServerService.ServerServiceClient client;
        private string serverUrl;

        public ClientLogic(string host) {

            serverUrl = host;
            AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            channel = GrpcChannel.ForAddress(host);
            client = new ServerService.ServerServiceClient(channel);
        }

        public string Read(string partitionId, string objectId, string serverId) {

            ReadReply reply = client.Read(new ReadRequest
            {
                ObjectId = objectId,
                PartitionId = partitionId
            });


            //ignore my broken pipe it still works
            return reply.ObjectValue.ToString();
        }
        public bool Write(string partitionId, string objectId, string value) {

            //string masterServer = config.findMServerByPartition(partitionId);
           /* if (!serverUrl.Equals(masterServer))
            {
                changeServer(masterServer);
            }*/

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

    }
}
