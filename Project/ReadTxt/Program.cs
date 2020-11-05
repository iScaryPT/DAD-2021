using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReadTxt
{
    class ParseTxt
    {
        static void Main(string[] args)
        {

            string text = System.IO.File.ReadAllText(@"C:\Users\heteronimo\Documents\GitHub\sample_pm_script.txt");

            // Display the file contents to the console. Variable text is a string.
            System.Console.WriteLine("Contents of WriteText.txt = {0}", text);

            StreamReader reader = File.OpenText(@"C:\Users\heteronimo\Documents\GitHub\sample_pm_script.txt");
            string lineb;
            int partitionsN = 0;
            int numberOfS;
            string partName;
            List<Partitions> allpartServers = new List<Partitions>();
            List<Server> allServers = new List<Server>();
            ArrayList commands = new ArrayList();
            while ((lineb = reader.ReadLine()) != null)
            {
                string[] linecontent = lineb.Split(" ");
                //string myInteger = linecontent[0];   // Here's your integer.
                //Console.WriteLine("\t" + myInteger);
                if (linecontent[0].Equals("ReplicationFactor"))
                {
                    Console.WriteLine(linecontent[0]);
                    partitionsN = int.Parse(linecontent[1]);
                }
                if (linecontent[0].Equals("Partition") && partitionsN != 0)
                {
                    //Console.WriteLine(linecontent[0]);
                    numberOfS = int.Parse(linecontent[1]);
                    partName = linecontent[2];
                    for (int i = 0; i < numberOfS; i++)
                    {
                        allpartServers.Add(new Partitions() { Name = partName, Servers = linecontent[3 + i] });
                    }
                }
                if (linecontent[0].Equals("Server"))
                {
                    //Console.WriteLine(linecontent[0]);
                    allServers.Add(new Server() { Name = linecontent[1], Location = linecontent[2], Mindelay = int.Parse(linecontent[3]), Maxdelay = int.Parse(linecontent[4]) });
                }
                else
                {
                    commands.Add(linecontent);
                }

            }

            foreach (Partitions p in allpartServers)
            {
                Console.WriteLine(p.Name);
            }

            Console.WriteLine();
            foreach (Server s in allServers)
            {
                Console.WriteLine(s.Name);
            }

        }
    }
    class Partitions
    {
        public string Name { get; set; }

        public string Servers { get; set; }
    }

    class Server
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public int Mindelay { get; set; }
        public int Maxdelay { get; set; }
    }
}

