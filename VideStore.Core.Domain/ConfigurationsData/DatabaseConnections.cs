namespace VideStore.Domain.ConfigurationsData;
public class DatabaseConnections
{
    public string StoreConnection {get; set; } = null!;
    public string HangfireConnection { get; set; } = null!;
    public string RedisConnection { get; set; } = null!;


}