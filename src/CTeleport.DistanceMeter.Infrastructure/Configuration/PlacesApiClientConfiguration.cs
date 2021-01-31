namespace CTeleport.DistanceMeter.Infrastructure.Configuration
{
    public class PlacesApiClientConfiguration
    {
        public string ApiName { get; set; }
        public string BaseUri { get; set; }
        public int RetryCount { get; set; }
        public int BackoffPower { get; set; }
    }
}