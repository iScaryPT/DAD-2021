using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ReadTxt
{
    class ParseTxt
    {
        static void Main(string[] args)
        {

            StreamReader reader = File.OpenText(args[0]);
            int partitionsN = 0;
            List<string> partition_info = new List<string>();
            List<string> server_info = new List<string>();
            List<string> general_cmds = new List<string>();
            Dictionary<string, string> parsedServerInfo = new Dictionary<string, string>();

            List<Process> serverProcesses = new List<Process>();
            List<Process> clientProcesses = new List<Process>();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] linecontent = line.Split(" ");

                if (linecontent[0].Equals("ReplicationFactor"))
                {
                    partitionsN = int.Parse(linecontent[1]);
                }
                else if (linecontent[0].Equals("Partition"))
                {
                   partition_info.Add(line);
                }
                else if (linecontent[0].Equals("Server"))
                {
                    server_info.Add(line);
                }
                else
                {
                    general_cmds.Add(line);
                }
            }
            
            foreach(string info in server_info)
            {
                string[] parsed = info.Split(" ");
                string final = "";
                string master = "";
                string parts = "";

                final += parsed[1] + "|" + parsed[2] + "|";

                foreach(string partition in partition_info)
                {
                    string[] partsplit = partition.Split(" ");
                    if (partsplit[3].Equals(parsed[1]))
                    {
                        master += partsplit[2] + ",";
                        parts += partsplit[2] + ",";
                    } else if (partsplit.Contains(parsed[1]))
                    {
                        parts += partsplit[2] + ",";
                    }
                }

                final += ((master.Length == 0) ? "null" : master) + "|" + parts + "|" + parsed[3] + "|" + parsed[4];

                parsedServerInfo[parsed[1]] = final; 
            }

            foreach(string server in parsedServerInfo.Values)
            {
                string cmdArgs = parsedServerInfo[server.Split("|")[0]];
                foreach(string s in parsedServerInfo.Values)
                {
                    if (!s.Split("|")[0].Equals(server.Split("|")[0])){
                        cmdArgs += " " + s;
                    }
                }

                //Console.WriteLine($"Creating process {server} with args: {cmdArgs}");
                serverProcesses.Add(new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "..\\..\\..\\..\\Server\\bin\\Debug\\netcoreapp3.1\\Server.exe",
                        Arguments = cmdArgs,
                        UseShellExecute = true,
                        CreateNoWindow = false
                    }
                }); 
            }

            foreach (Process p in serverProcesses)
                p.Start();

            foreach( string command in general_cmds)
            {
                string[] splitted = command.Split(" ");

                switch (splitted[0])
                {
                    case "Client":
                        {
                            string cmdArgs = splitted[1] + " ";
                            cmdArgs += splitted[2] + " ";
                            cmdArgs += splitted[3] + " ";
                            foreach (string s in parsedServerInfo.Values)
                                cmdArgs += s + " ";
                            Console.WriteLine(cmdArgs);
                            clientProcesses.Add(new Process
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = "..\\..\\..\\..\\Client\\bin\\Debug\\netcoreapp3.1\\Client.exe",
                                    Arguments = cmdArgs,
                                    UseShellExecute = true,
                                    CreateNoWindow = false,
                                    RedirectStandardOutput = false
                                }
                            });

                            clientProcesses.Last().Start();

                            //Console.WriteLine(clientProcesses.Last().StandardOutput.ReadToEnd());
                            break;
                        }
                    default:
                        break;
                }

            }

        }
    }

}

