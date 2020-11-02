using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ConfigStorageSP
{
    class ConfigStorage
    {

        JObject config;

        public ConfigStorage(string file)
        {
            config = JObject.Parse(File.ReadAllText(file));
            Console.WriteLine(config);
        }

        public JToken getServers()
        {   
            
            return config["Servers"];
        }

        public string findServerById(string id)
        {
            foreach(var server in config["Servers"])
            {
                if (server["Id"].ToString().Equals("s1"))
                    return server["Url"].ToString();
            }

            return "";
        }

        public string findMServerByPartition(int partition)
        {
            foreach (var server in config["Servers"])
            {
                if (server["Master"].ToObject<int []>().Contains<int>(partition))
                    return server["Url"].ToString();
            }

            return "";
        }

        public string findRandomServerByPartition(int partition)
        {
            List<string> res = new List<string>();

            foreach (var server in config["Servers"])
            {
                if (server["Partitions"].ToObject<int[]>().Contains<int>(partition))
                    res.Add(server["Url"].ToString());
            }

            return res.ElementAt((new Random()).Next(0, res.Count-1));
        }

        public void takeServer(string serverId)
        {
            foreach (var server in config["Servers"])
            {
                if (server["Id"].ToString().Equals(serverId)) { 
                    server["Taken"] = 1;
                    Console.Write("server id  " );
                    Console.WriteLine(server["Id"].ToString());
                }
                else
                {
                    Console.Write(server["Id"].ToString());
                    Console.Write("not equal to ");
                    Console.WriteLine(serverId);
                    //o problema nao está aqui mas pro alguma razao ele acha que o taken é 1 em vez de 0
                    //RESOLVED
                }

            }

            File.WriteAllText("teste.json", config.ToString());
        }

    }
}
