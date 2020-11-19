using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace PuppetMasterSP
{
    class PuppetMaster
    {
        StreamReader reader;
        List<string> partition_info;
        List<string> server_info;
        List<string> general_cmds;
        Dictionary<string, string> parsedServerInfo;
        //List<Process> serverProcesses;
        Dictionary<string, Process> serverProcesses;
        List<Process> clientProcesses;
        int partitionsN;
        object internalLock;
        object internalLock2;

        public PuppetMaster(string file)
        {
            this.reader = File.OpenText(file);
            this.partition_info = new List<string>();
            this.server_info = new List<string>();
            this.general_cmds = new List<string>();
            this.parsedServerInfo = new Dictionary<string, string>();
            this.serverProcesses = new Dictionary<string, Process>();
            this.clientProcesses = new List<Process>();
            this.partitionsN = 0;
            this.internalLock = new object();
            this.internalLock2 = new object();
        }

        public void ParseCommands()
        {
            string line;
            while ((line = this.reader.ReadLine()) != null)
            {
                string[] linecontent = line.Split(" ");

                if (linecontent[0].Equals("ReplicationFactor"))
                {
                    this.partitionsN = int.Parse(linecontent[1]);
                }
                else if (linecontent[0].Equals("Partition"))
                {
                    this.partition_info.Add(line);
                }
                else if (linecontent[0].Equals("Server"))
                {
                    this.server_info.Add(line);
                }
                else
                {
                    this.general_cmds.Add(line);
                }
            }
        }

        public void BuildServerInfo()
        {
            foreach (string info in this.server_info)
            {
                string[] parsed = info.Split(" ");
                string final = "";
                string master = "";
                string parts = "";

                final += parsed[1] + "|" + parsed[2] + "|";

                foreach (string partition in this.partition_info)
                {
                    string[] partsplit = partition.Split(" ");
                    if (partsplit[3].Equals(parsed[1]))
                    {
                        master += partsplit[2] + ",";
                        parts += partsplit[2] + ",";
                    }
                    else if (partsplit.Contains(parsed[1]))
                    {
                        parts += partsplit[2] + ",";
                    }
                }

                final += ((master.Length == 0) ? "null" : master) + "|" + parts + "|" + parsed[3] + "|" + parsed[4];

                this.parsedServerInfo[parsed[1]] = final;
            }
        }


        public void StartServersAsync()
        {
            //Task.Run(() => Parallel.ForEach(this.parsedServerInfo.Values, server => runServerAsync(server)));
            Task.WhenAll(this.parsedServerInfo.Values.Select(server => Task.Run(() => runServerAsync(server))));
        }

        public Task<object> runServerAsync(string server)
        {
            /* Async Test
            for (int i = 0; i < 999999999; i++)
                ;
            */

            string cmdArgs = this.parsedServerInfo[server.Split("|")[0]];
            foreach (string s in this.parsedServerInfo.Values)
            {
                if (!s.Split("|")[0].Equals(server.Split("|")[0]))
                {
                    cmdArgs += " " + s;
                }
            }

            Process p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "..\\..\\..\\..\\Server\\bin\\Debug\\netcoreapp3.1\\Server.exe",
                    Arguments = cmdArgs,
                    UseShellExecute = true,
                    CreateNoWindow = false
                }
            };

            p.Start();

            lock (this.internalLock)
            {
                this.serverProcesses[server.Split("|")[0]] = p;
            }

            

            return Task.FromResult(new object());
        }

        public void crashAsync(string server)
        {
            this.serverProcesses[server].Kill();
            lock (this.internalLock)
            {
                this.serverProcesses.Remove(server);
            }
        }

        public Task<object> runClientAsync(string[] splitted)
        {
            string cmdArgs = splitted[1] + " ";
            cmdArgs += splitted[2] + " ";
            cmdArgs += splitted[3] + " ";
            foreach (string s in parsedServerInfo.Values)
                cmdArgs += s + " ";
            //Console.WriteLine(cmdArgs);

            Process p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "..\\..\\..\\..\\Client\\bin\\Debug\\netcoreapp3.1\\Client.exe",
                    Arguments = cmdArgs,
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    RedirectStandardOutput = false
                }
            };

            p.Start();

            lock (internalLock2)
            {
                clientProcesses.Add(p);
            }

            return Task.FromResult(new object());
        }

        public void FreezeAsync(string serverid)
        {
            string host = this.parsedServerInfo[serverid].Split("|")[1];

            AppContext.SetSwitch(
                   "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            
            GrpcChannel channel = GrpcChannel.ForAddress(host);
            ServerService.ServerServiceClient client = new ServerService.ServerServiceClient(channel);

            client.Freeze(new FreezeRequest { });

            channel.ShutdownAsync().Wait();

        }

        public void UnFreezeAsync(string serverid)
        {
            string host = this.parsedServerInfo[serverid].Split("|")[1];

            AppContext.SetSwitch(
                   "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            GrpcChannel channel = GrpcChannel.ForAddress(host);
            ServerService.ServerServiceClient client = new ServerService.ServerServiceClient(channel);

            client.UnFreeze(new UnFreezeRequest { });

            channel.ShutdownAsync().Wait();

        }

        public void executeCmdsAsync()
        {
            foreach (string command in this.general_cmds)
            {
                this.execute(command);
            }
            
        }

        public void showStatus()
        {
            foreach(string server in this.parsedServerInfo.Values)
            {
                string host = server.Split("|")[1];

                AppContext.SetSwitch(
                       "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

                GrpcChannel channel = GrpcChannel.ForAddress(host);
                ServerService.ServerServiceClient client = new ServerService.ServerServiceClient(channel);

                try
                {
                    client.Status(new StatusRequest { });
                    channel.ShutdownAsync().Wait();
                } catch(Exception)
                {
                    Console.WriteLine($"Server {host} not responding.");
                }
            }
        }

        public void execute(string command)
        {
            string[] splitted = command.Split(" ");

            switch (splitted[0])
            {
                case "Client":
                    {
                        Task.Run(() => this.runClientAsync(splitted));
                        break;
                    }
                case "Crash":
                    {
                        Task.Run(() => this.crashAsync(splitted[1]));
                        break;
                    }
                case "Wait":
                    {
                        Thread.Sleep(Int32.Parse(splitted[1]));
                        break;
                    }
                case "Freeze":
                    {
                        Task.Run(() => this.FreezeAsync(splitted[1]));
                        break;
                    }
                case "UnFreeze":
                    {
                        Task.Run(() => this.UnFreezeAsync(splitted[1]));
                        break;
                    }
                case "Status":
                    {
                        Task.Run(() => this.showStatus());
                        break;
                    }
                default:
                    break;
            }
        }

        static void Main(string[] args)
        {

            PuppetMaster puppetM = new PuppetMaster(args[0]);
            
            //Not Async
            puppetM.ParseCommands();
            puppetM.BuildServerInfo();


            //Async
            puppetM.StartServersAsync();
            puppetM.executeCmdsAsync();

            while (true)
            {
                string cmd = Console.ReadLine();
                puppetM.execute(cmd);

            }
            
        }
    }

}

