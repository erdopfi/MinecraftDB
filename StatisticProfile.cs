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
                    var idModString = idProperty.Name.Split(":");
                    var mod = idModString[0];
                    var id = idModString[1];
                    var value = idProperty.Value.ToObject<int>();

                    Console.WriteLine(Uuid + " - " + category + " - " + idModString + " - " + value);
                    string deleteQuery =
                        $"delete from Stat where category='{category}' and world='{WorldName}' and mod='{mod}' and id='{id}' and uuid='{Uuid}'";
                    var deleteCmd = new SqlCommand(deleteQuery, sqlConnection);
                    string insertQuery = $"insert into Stat values('{category}', '{WorldName}', '{mod}', '{id}', '{Uuid}', {value})";
                    var insertCmd = new SqlCommand(insertQuery, sqlConnection);
                    deleteCmd.ExecuteNonQuery();
                    insertCmd.ExecuteNonQuery();
                }
            }
        }
    }
}