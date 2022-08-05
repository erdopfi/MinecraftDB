namespace MinecraftDB
{
    public class WorldConfiguration
    {
        public string WorldPathDirectory { get; }

        public int RefreshAmount { get; }

        public int RefreshCooldown { get; }

        public int StatisticsVersion { get; }

        public string SqlConnection { get; }

        public WorldConfiguration(string worldPathDirectory, int refreshAmount, int refreshCooldown, int statisticsVersion, string sqlConnection)
        {
            WorldPathDirectory = worldPathDirectory;
            RefreshAmount = refreshAmount;
            RefreshCooldown = refreshCooldown;
            StatisticsVersion = statisticsVersion;
            SqlConnection = sqlConnection;
        }
    }
}