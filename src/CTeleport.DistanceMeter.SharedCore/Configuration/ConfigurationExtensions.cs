namespace CTeleport.DistanceMeter.SharedCore.Configuration
{
    using Microsoft.Extensions.Configuration;

    public static class ConfigurationExtensions
    {
        public static T GetSection<T>(this IConfiguration configuration)
        {
            var type = typeof(T);

            return configuration.GetSection(type.Name).Get<T>();
        }
    }
}