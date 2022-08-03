using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MinecraftDB
{
    public class StatsCreator
    {
        private string? _worldFilePath;
        private string StatsFolder => _worldFilePath + "\\stats";

        public StatsCreator(string? worldFilePath)
        {
            _worldFilePath = worldFilePath;
        }

        public List<StatisticProfile> CreateAll()
        {
            var jObject = new List<StatisticProfile>();

            foreach(var file in Directory.GetFiles(StatsFolder))
            {
                var uuid = file.Split("\\")[^1].Replace(".json", "");
                jObject.Add(CreateProfile(uuid,File.ReadAllText(file)));
            }
            return jObject;
        }
        
        public StatisticProfile? Create(string uuid)
        {
            string file = StatsFolder + "\\" + uuid + ".json";
            
            if (!File.Exists(file))
                return null;

            var profile = CreateProfile( uuid,File.ReadAllText(file));
            return profile;
        }

        private StatisticProfile CreateProfile(string uuid, string rawStringData)
        {
            var jObject = JsonConvert.DeserializeObject<JObject>(rawStringData);
            return new StatisticProfile(uuid, (JObject)jObject!.Property("stats")?.Value!);
        }
    }
}