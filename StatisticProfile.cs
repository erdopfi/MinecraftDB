using System;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace MinecraftDB
{
    public struct StatisticProfile
    {
        public readonly string Uuid;
        public readonly string WorldName;
        public readonly JObject Stats;
        

        public StatisticProfile(string uuid, string worldName, JObject stats)
        {
            Uuid = uuid;
            WorldName = worldName;
            Stats = stats;
        }

        public void WriteDatabase(SqlConnection sqlConnection)
        {
            foreach (var categoryProperty in Stats.Properties())
            {
                var categoryValue = categoryProperty.Value;
                if (categoryValue.Type != JTokenType.Object) continue;
                    
                var categoryObject = (JObject) categoryValue;
                var category = categoryProperty.Name;
                    
                foreach (var idProperty in categoryObject.Properties())
                {
                    var id = idProperty.Name;
                    var value = idProperty.Value.ToObject<int>();

                    Console.WriteLine(Uuid + " - " + category + " - " + id + " - " + value);
                        
                    string query = $"insert into Stat values('{category}', '{id}', '{WorldName}', '{Uuid}', {value})";
                    var cmd = new SqlCommand(query, sqlConnection);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}