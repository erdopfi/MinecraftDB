using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using MySqlConnector;
using Newtonsoft.Json.Linq;

namespace MinecraftDB
{
    public struct StatisticProfile
    {
        public readonly string Uuid;
        public readonly string WorldName;
        public readonly JObject Stats;
        
        private const string SelectQuery = 
            "select sum(value) from profile where category = @category and world = @world and type = @type and id = @id and uuid = @uuid";
        private const string InsertQuery = 
            "insert into profile values(@category, @world, @type, @id, @uuid, @value)";
        private const string UpdateQuery =
            "update profile set value = @value where category = @category and world = @world and type = @type and id = @id and uuid = @uuid";

        public StatisticProfile(string uuid, string worldName, JObject stats)
        {
            Uuid = uuid;
            WorldName = worldName;
            Stats = stats;
        }

        public void WriteDatabase(MySqlConnection sqlConnection)
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
                    var type = idModString[0];
                    var id = idModString[1];
                    var value = idProperty.Value.ToObject<int>();
                    
                    var selectCommand = new MySqlCommand(SelectQuery, sqlConnection);
                    selectCommand.Parameters.Add(new MySqlParameter("@category", MySqlDbType.VarChar, 100) {Value = category});
                    selectCommand.Parameters.Add(new MySqlParameter("@world", MySqlDbType.VarChar, 50){Value = WorldName});
                    selectCommand.Parameters.Add(new MySqlParameter("@type", MySqlDbType.VarChar, 50){Value = type});
                    selectCommand.Parameters.Add(new MySqlParameter("@id", MySqlDbType.VarChar, 50){Value = id});
                    selectCommand.Parameters.Add(new MySqlParameter("@uuid", MySqlDbType.VarChar, 36){Value = Uuid});

                    using (var reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader[0] is DBNull)
                            {
                                reader.Close();
                                
                                var insertCommand = new MySqlCommand(InsertQuery, sqlConnection);

                                insertCommand.Parameters.Add(new MySqlParameter("@category", MySqlDbType.VarChar, 100) {Value = category});
                                insertCommand.Parameters.Add(new MySqlParameter("@world", MySqlDbType.VarChar, 50){Value = WorldName});
                                insertCommand.Parameters.Add(new MySqlParameter("@type", MySqlDbType.VarChar, 50){Value = type});
                                insertCommand.Parameters.Add(new MySqlParameter("@id", MySqlDbType.VarChar, 50){Value = id});
                                insertCommand.Parameters.Add(new MySqlParameter("@uuid", MySqlDbType.VarChar, 36){Value = Uuid});
                                insertCommand.Parameters.Add(new MySqlParameter("@value", MySqlDbType.Int64, 0){Value = value});

                                insertCommand.Prepare();
                                insertCommand.ExecuteNonQuery();
                                
                                Console.WriteLine($"{category} - {WorldName} - {type} - {id} - {Uuid}: {value}");
                            }
                            else
                            {
                                reader.Close();
                                
                                var updateCommand = new MySqlCommand(UpdateQuery, sqlConnection);
                                updateCommand.Parameters.Add(new MySqlParameter("@value", MySqlDbType.Int64, 36){Value = value});
                                updateCommand.Parameters.Add(new MySqlParameter("@category", MySqlDbType.VarChar, 100) {Value = category});
                                updateCommand.Parameters.Add(new MySqlParameter("@world", MySqlDbType.VarChar, 50){Value = WorldName});
                                updateCommand.Parameters.Add(new MySqlParameter("@type", MySqlDbType.VarChar, 50){Value = type});
                                updateCommand.Parameters.Add(new MySqlParameter("@id", MySqlDbType.VarChar, 50){Value = id});
                                updateCommand.Parameters.Add(new MySqlParameter("@uuid", MySqlDbType.VarChar, 36){Value = Uuid});
                                
                                updateCommand.Prepare();
                                updateCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }
    }
}