using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ClientLogicSP;

namespace ClientSP
{

    class Client
    {

        private ClientLogic client;

        private bool is_repeating = false;
        private int repeat_num = 1;
        private List<string> list_cmds;

        string name;
        string url;

        public Client(string name, string url, string[] serverurls) {

            this.name = name;
            this.url = url;

            client = new ClientLogic(serverurls);
        }
        public int RepeatNum()
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
            
            Console.WriteLine($"Read response: {client.Read(commandArgs[1], commandArgs[2], commandArgs[3])}");
        }
        public void Write(string[] commandArgs) {
            if (commandArgs.Length < 4)
            {
                Console.WriteLine("Command error: invalid input...");
                Console.WriteLine("Definition: write <partition_id> <object_id> <value>");
                return;
            }

            string value = "";
            for (var i = 3; i < commandArgs.Length; i++)
                value += commandArgs[i] + (i == commandArgs.Length - 1 ? "" : " ");
            value = value.Replace("\"", "");
            Console.WriteLine($"Write Status: {client.Write(commandArgs[1], commandArgs[2], value)}");
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
                string serverId = commandArgs[1];
                Console.WriteLine(client.listServer(serverId));
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
            Console.WriteLine(client.listGlobal());
        }
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
                Console.WriteLine($"Waiting {millSec / 1000} seconds ...");
                Thread.Sleep(millSec);
            } catch(FormatException e)
            {
                Console.WriteLine(e.Message);
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
                this.list_cmds = new List<string>();

            } catch(FormatException e)
            {
                Console.WriteLine(e.Message);
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
            Console.WriteLine("End of Begin-Repeat");
            this.is_repeating = false;

            for (var i = 0; i < this.repeat_num; i++)
            {
                foreach(string cmd in this.list_cmds)
                {
                    string parsedCmd = cmd.Replace("$i", (i + 1).ToString());
                    this.parseCommand(parsedCmd);
                }
                
            }
        }
        public void parseCommand(string command)
        {
            string[] commandArgs = command.Split(" ");

            //to filter incoming begin-repeats
            if (commandArgs[0].Equals("begin-repeat") && this.is_repeating)
            {
                return;
            }

            if (this.is_repeating && !commandArgs[0].Equals("end-repeat"))
            {
                this.list_cmds.Add(command);
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
                default:
                    Console.WriteLine("Command not found!");
                    break;
            }
        }

        static void Main(string[] args)
        {
            string username = args[0];
            string clienturl = args[1];
            string scriptfile = args[2];

            Client client = new Client(username, clienturl, args);

            //To Read from Script
            try
            {
                StreamReader reader = File.OpenText(scriptfile);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    client.parseCommand(line);
                }

            } catch(FileNotFoundException)
            {
                Console.WriteLine("File not found or not provided.");
            }

            //To Read from Terminal
            while (true)
            {
                string command = Console.ReadLine();

                client.parseCommand(command);
                   
            }


        }

    }
}
