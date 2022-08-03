using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace MinecraftDB
{
    public class WorldDatabase
    {
        private const string ConnectionString = "Data Source = DESKTOP-ME8DQVU;Initial Catalog=MinecraftDB;Integrated Security=true";

        private readonly string _worldFilePath;
        private readonly SqlConnection _con;
        private readonly string _worldName;
        private int _refreshAmount;
        private int _refreshCooldown;

        public WorldDatabase(string worldFilePath, int refreshAmount, int refreshCooldown, string databaseConnection)
        {
            _worldFilePath = worldFilePath;
            _worldName = worldFilePath.Split("\\")[^1];
            _refreshAmount = refreshAmount;
            _refreshCooldown = refreshCooldown;

            _con = new SqlConnection(databaseConnection);
            _con.Open();
        }

        public void Start()
        {
            var creator = new StatsCreator(_worldFilePath);
            var profiles = creator.CreateAll();
            
            foreach (var profile in profiles)
            {
                foreach (var categoryProperty in profile.Stats.Properties())
                {
                    var categoryValue = categoryProperty.Value;
                    if (categoryValue.Type != JTokenType.Object) continue;
                    
                    var categoryObject = (JObject) categoryValue;
                    var category = categoryProperty.Name;
                    
                    foreach (var idProperty in categoryObject.Properties())
                    {
                        var id = idProperty.Name;
                        var value = idProperty.Value.ToObject<int>();

                        Console.WriteLine(profile.Uuid + " - " + category + " - " + id + " - " + value);
                        
                        string query = $"insert into Stat values('{category}', '{id}', '{_worldName}', '{profile.Uuid}', {value})";
                        var cmd = new SqlCommand(query, _con);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}