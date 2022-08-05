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

        private readonly string _worldPathDirectory;
        private readonly string _worldName;
        private int _refreshAmount;
        private int _refreshCooldown;
        private int _statisticsVersion;
        
        private readonly SqlConnection _con;

        public WorldDatabase(WorldConfiguration configuration)
        {
            _worldPathDirectory = configuration.WorldPathDirectory;
            _worldName = _worldPathDirectory.Split("\\")[^1];
            _refreshAmount = configuration.RefreshAmount;
            _refreshCooldown = configuration.RefreshCooldown;
            _statisticsVersion = configuration.StatisticsVersion;
            
            Console.WriteLine(_worldName);
            Console.WriteLine(_worldPathDirectory);

            string worldIdFile = _worldPathDirectory + "\\world.id";
            if (File.Exists(worldIdFile))
                _worldName = File.ReadAllText(worldIdFile);
            else
            {
                _worldName = Guid.NewGuid().ToString();
                using StreamWriter sw = File.CreateText(worldIdFile);
                sw.Write(_worldName);
                Console.WriteLine("World has no id, generating one");
            }

            _con = new SqlConnection(configuration.SqlConnection);
            _con.Open();
        }

        public WorldDatabase(string worldPathDirectory, int refreshAmount, int refreshCooldown, int statisticsVersion, string databaseConnection)
        {
            _worldPathDirectory = worldPathDirectory;
            _worldName = worldPathDirectory.Split("\\")[^1];
            _refreshAmount = refreshAmount;
            _refreshCooldown = refreshCooldown;
            _statisticsVersion = statisticsVersion;

            _con = new SqlConnection(databaseConnection);
            _con.Open();
        }

        public void Start()
        {
            var profiles = StatsCreator.CreateAll(_worldPathDirectory, _worldName);
            
            foreach (var profile in profiles)
            {
                profile.WriteDatabase(_con);
            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}