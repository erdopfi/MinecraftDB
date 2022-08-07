using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MinecraftDB
{
    internal static class Program
    {

        private const string ConfigFile = "config.json";
        
        private static void Main(string[] args)
        {
            if (!File.Exists(ConfigFile))
            {
                Console.WriteLine("Config file not found, generating blank config file, needs a restart");
                var blankConfigurationString =
                    JsonSerializer.Serialize(new[]{new WorldConfiguration("path", -1, 60, 1, "path")});

                using StreamWriter sw = File.CreateText(ConfigFile);
                sw.Write(blankConfigurationString);

                return;
            }
            string file = File.ReadAllText(ConfigFile);
            var worldConfigurations = JsonSerializer.Deserialize<List<WorldConfiguration>>(file);
            if (worldConfigurations == null)
            {
                Console.WriteLine("No world configurations found in config");
                return;
            }
            
            foreach (var worldConfiguration in worldConfigurations)
            {
                var world = new WorldDatabase(worldConfiguration);
                world.Start();
            }
        }
    }
}

