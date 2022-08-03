using Newtonsoft.Json.Linq;

namespace MinecraftDB
{
    public struct StatisticProfile
    {
        public readonly string Uuid;
        public readonly JObject Stats;

        public StatisticProfile(string uuid, JObject stats)
        {
            Uuid = uuid;
            Stats = stats;
        }
    }
}