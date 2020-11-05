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
        }


        public string findServerById(string id)
        {
            foreach(var server in config["Servers"])
            {
                if (server["Id"].ToObject<string>().Equals(id))
                    return server["Url"].ToString();
            }

            return "";
        }

        public string findMServerByPartition(string partition)
        {
            foreach (var server in config["Servers"])
            {
                if (server["Master"].ToObject<string []>().Contains(partition))
                    return server["Url"].ToString();
            }

            return "";
        }

        public string findRandomServerByPartition(string partition)
        {
            List<string> res = new List<string>();

            foreach (var server in config["Servers"])
            {
                if (server["Partitions"].ToObject<string[]>().Contains(partition))
                    res.Add(server["Url"].ToString());
            }

            return res.ElementAt((new Random()).Next(0, res.Count-1));
        }

    }
}
