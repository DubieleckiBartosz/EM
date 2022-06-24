namespace EventManagement.Application.Settings
{
    public class RedisConnection
    {
        public const string EMRedisSection = "EMRedisCacheSettings";
        public bool Enabled { get; set; }
        public string EMRedisConnection { get; set; }
        public int DefaultTimeInHours { get; set; }
    }
}
