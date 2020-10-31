using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using ClientLogicSP;
using ConfigStorageSP;

namespace ClientSP
{
    class Client
    {

        private ClientLogic client;
        private readonly ConfigStorage config;
        private bool is_repeating = false;
        private int repeat_num = 1;

        public Client(string configFile) {
            config = new ConfigStorage(configFile);
            client = new ClientLogic(config.findMServerByPartition(1), config);
        }

        public int repeatNum()
        {
            return this.repeat_num;
        }

        public void Read(string[] commandArgs) {
            if(commandArgs.Length != 4)
            {
                Console.WriteLine("Command error: invalid input...");
                Console.WriteLine("Definition: read <partition_id> <object_id> <server_id>");
                return;
            }

            try
            {
                //Console.WriteLine("Read OK");
                int partitionId = int.Parse(commandArgs[1]);
                int objectId = int.Parse(commandArgs[2]);
                int serverId = int.Parse(commandArgs[3]);
                Console.WriteLine(client.Read(partitionId, objectId, serverId));
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Command error: invalid number format...");
                Console.WriteLine("Definition: read <partition_id> <object_id> <server_id>");
            }
        }

        public void Write(string[] commandArgs) {
            if (commandArgs.Length < 4)
            {
                Console.WriteLine("Command error: invalid input...");
                Console.WriteLine("Definition: write <partition_id> <object_id> <value>");
                return;
            }

            try
            {
                //Console.WriteLine("Write OK");
                int partitionId = int.Parse(commandArgs[1]);
                int objectId = int.Parse(commandArgs[2]);
                string value = "";
                for (var i = 3; i < commandArgs.Length; i++)
                    value += commandArgs[i] + (i == commandArgs.Length - 1 ? "" : " ");
                value = value.Replace("\"", "");
                Console.WriteLine(client.Write(partitionId, objectId, value));
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Command error: invalid number format...");
                Console.WriteLine("Definition: write <partition_id> <object_id> <value>");
            }
        }

        public void ListServer(string[] commandArgs) {
            if (commandArgs.Length != 2)
            {
                Console.WriteLine("Command error: invalid input...");
                Console.WriteLine("Definition: listServer <server_id>");
                return;
            }

            try
            {
                //Console.WriteLine("List Server OK");
                int serverId = int.Parse(commandArgs[1]);
                //Call some function
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Command error: invalid number format...");
                Console.WriteLine("Definition: listServer <server_id>");
            }

        }

        public void ListGlobal(string[] commandArgs) {
            if (commandArgs.Length != 1)
            {
                Console.WriteLine("Command error: invalid input...");
                Console.WriteLine("Definition: listGlobal");
                return;
            }
            //Console.WriteLine("List Global OK");
        }

        //TODO exception handling for thread.sleep()
        public void Wait(string[] commandArgs) {
            if (commandArgs.Length != 2)
            {
                Console.WriteLine("Command error: invalid input...");
                Console.WriteLine("Definition: wait <num_milliseconds>");
                return;
            }

            try 
            { 
                int millSec = int.Parse(commandArgs[1]);
                Thread.Sleep(millSec);
            } catch(FormatException e)
            {   
                Console.WriteLine("Command error: invalid number format of milliseconds...");
            }
            
        }

        public void Begin_repeat(string[] commandArgs) {
            if (commandArgs.Length != 2)
            {
                Console.WriteLine("Command error: invalid input...");
                Console.WriteLine("Definition: begin-repeat <num_repeats>");
                return;
            }
            Console.WriteLine("Begin Repeat OK");
            try
            {
                this.is_repeating = true;
                this.repeat_num = int.Parse(commandArgs[1]);

            } catch(FormatException e)
            {
                Console.WriteLine("Command error: invalid number format of milliseconds...");
            }
        }

        public void End_repeat(string[] commandArgs) {
            if (commandArgs.Length != 1)
            {
                Console.WriteLine("Command error: invalid input...");
                Console.WriteLine("Definition: end-repeat");
                return;
            }
            this.is_repeating = false;
            this.repeat_num = 1;
            //Console.WriteLine("End Repeat OK");
        }

        public void parseCommand(string command)
        {
            string[] commandArgs = command.Split(" ");

            //to filter incoming begin-repeats
            if (commandArgs[0].Equals("begin-repeat") && this.is_repeating)
            {
                return;
            }

            switch (commandArgs[0])
            {
                case "read":
                    this.Read(commandArgs);
                    break;
                case "write":
                    this.Write(commandArgs);
                    break;
                case "listServer":
                    this.ListServer(commandArgs);
                    break;
                case "listGlobal":
                    this.ListGlobal(commandArgs);
                    break;
                case "wait":
                    this.Wait(commandArgs);
                    break;
                case "begin-repeat":
                    this.Begin_repeat(commandArgs);
                    break;
                case "end-repeat":
                    this.End_repeat(commandArgs);
                    break;
                case null:
                    break;
                default:
                    Console.WriteLine("Command not found!");
                    break;
            }
        }

        static void Main(string[] args)
        {

            Client client = new Client("teste.json");

            while (true)
            {
                string command = Console.ReadLine();

                for(var i = 0; i < client.repeatNum(); i++)
                {
                    string newCmd = command.Replace("$i", (i + 1).ToString());
                    client.parseCommand(newCmd);
                    Console.WriteLine("exec: " + newCmd);
                }
                   
            }
        }

    }
}
