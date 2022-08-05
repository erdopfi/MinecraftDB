using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MinecraftDB
{
    public static class StatsCreator
    {
        public static List<StatisticProfile> CreateAll(string worldPathDirectory, string worldName)
        {
            var jObject = new List<StatisticProfile>();

            foreach(var file in Directory.GetFiles(worldPathDirectory + "\\Stats"))
            {
                var uuid = file.Split("\\")[^1].Replace(".json", "");
                jObject.Add(CreateProfile(uuid,File.ReadAllText(file), worldName));
            }
            return jObject;
        }
        
        public static StatisticProfile? Create(string uuid, string worldPathDirectory, string worldName)
        {
            string file = worldPathDirectory + "\\Stats\\" + uuid + ".json";
            
            if (!File.Exists(file))
                return null;

            var profile = CreateProfile( uuid,File.ReadAllText(file), worldName);
            return profile;
        }

        private static StatisticProfile CreateProfile(string uuid, string rawStringData, string worldName)
        {
            var jObject = JsonConvert.DeserializeObject<JObject>(rawStringData);
            return new StatisticProfile(uuid, worldName, (JObject)jObject!.Property("stats")?.Value!);
        }
    }
}