
using MySqlConnector;

namespace MinecraftDB
{
    public class WorldDatabase
    {
        private readonly string _worldPathDirectory;
        private readonly string _worldName;
        private int _refreshAmount;
        private int _refreshCooldownInSeconds;
        private int _statisticsVersion;

        private readonly MySqlConnection _con;

        public WorldDatabase(WorldConfiguration configuration)
        {
            _worldPathDirectory = configuration.WorldPathDirectory;
            _worldName = _worldPathDirectory.Split("\\")[^1];
            _refreshAmount = configuration.RefreshAmount;
            _refreshCooldownInSeconds = configuration.RefreshCooldown;
            _statisticsVersion = configuration.StatisticsVersion;

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

            _con = new MySqlConnection(configuration.SqlConnection);
            _con.Open();
        }

        public void Start()
        {
            Thread t = new(ProfileWriteThread);
            t.Start();
        }

        public void Stop()
        {
            _refreshAmount = 0;
            Console.WriteLine("Stopped world " + _worldName);
        }

        private void ProfileWriteThread()
        {
            while (_refreshAmount != 0)
            {
                var profiles = StatsCreator.CreateAll(_worldPathDirectory, _worldName);

                foreach (var profile in profiles)
                {
                    profile.WriteDatabase(_con);
                }

                string consoleOutput = _refreshAmount < 0
                    ? $"{_worldName} is waiting for {_refreshCooldownInSeconds} seconds - Infinite repeats left"
                    : $"{_worldName} is waiting for {_refreshCooldownInSeconds} seconds - {_refreshAmount} repeats left";
                Console.WriteLine(consoleOutput);
                _refreshAmount--;
                
                Thread.Sleep(_refreshCooldownInSeconds * 1000);
            }
        }
    }
}