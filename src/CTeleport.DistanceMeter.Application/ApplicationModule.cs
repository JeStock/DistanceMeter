namespace CTeleport.DistanceMeter.Application
{
    using Microsoft.Extensions.DependencyInjection;
    using Services;

    public static class ApplicationModule
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            return services.AddScoped<IDistanceMeasurementService, DistanceMeasurementService>();
        }
    }
}