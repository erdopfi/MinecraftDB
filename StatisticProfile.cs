using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace MinecraftDB
{
    public struct StatisticProfile
    {
        public readonly string Uuid;
        public readonly string WorldName;
        public readonly JObject Stats;
        
        private const string SelectQuery = 
            "select sum(value) from Stat where category = @category and world = @world and mod = @mod and id = @id and uuid = @uuid";
        private const string InsertQuery = 
            "insert into Stat values(@category, @world, @mod, @id, @uuid, @date, @value)";

        public StatisticProfile(string uuid, string worldName, JObject stats)
        {
            Uuid = uuid;
            WorldName = worldName;
            Stats = stats;
        }

        public void WriteDatabase(SqlConnection sqlConnection)
        {
            string dateString = DateTime.Now.ToString(CultureInfo.CurrentCulture);
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
                    
                    var selectCommand = new SqlCommand(SelectQuery, sqlConnection);
                    selectCommand.Parameters.Add(new SqlParameter("@category", SqlDbType.NChar, 100) {Value = category});
                    selectCommand.Parameters.Add(new SqlParameter("@world", SqlDbType.NChar, 50){Value = WorldName});
                    selectCommand.Parameters.Add(new SqlParameter("@mod", SqlDbType.NChar, 50){Value = mod});
                    selectCommand.Parameters.Add(new SqlParameter("@id", SqlDbType.NChar, 50){Value = id});
                    selectCommand.Parameters.Add(new SqlParameter("@uuid", SqlDbType.NChar, 36){Value = Uuid});
                    var dbValue = 0;
                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader[0] is DBNull)
                                dbValue = 0;
                            else
                                dbValue = (int) reader[0];

                            
                            if (value - dbValue == 0)
                            {
                                continue;
                            }
                        }
                    }

                    var deltaValue = value - dbValue;
                    
                    //Console.WriteLine($"{value} - {dbValue} - {value - deltaValue}");

                    Console.WriteLine(WorldName + " - " + Uuid + " - " + dateString + " - " + category + " - " + idProperty.Name + " - " + deltaValue);
                    var insertCmd = new SqlCommand(InsertQuery, sqlConnection);

                    insertCmd.Parameters.Add(new SqlParameter("@category", SqlDbType.NChar, 100) {Value = category});
                    insertCmd.Parameters.Add(new SqlParameter("@world", SqlDbType.NChar, 50){Value = WorldName});
                    insertCmd.Parameters.Add(new SqlParameter("@mod", SqlDbType.NChar, 50){Value = mod});
                    insertCmd.Parameters.Add(new SqlParameter("@id", SqlDbType.NChar, 50){Value = id});
                    insertCmd.Parameters.Add(new SqlParameter("@uuid", SqlDbType.NChar, 36){Value = Uuid});
                    insertCmd.Parameters.Add(new SqlParameter("@date", SqlDbType.SmallDateTime, 50){Value = dateString});
                    insertCmd.Parameters.Add(new SqlParameter("@value", SqlDbType.Int, 0){Value = deltaValue});

                    insertCmd.Prepare();
                    insertCmd.ExecuteNonQuery();
                }
            }
        }
    }
}