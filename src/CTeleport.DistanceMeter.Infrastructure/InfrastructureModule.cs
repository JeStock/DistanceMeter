namespace CTeleport.DistanceMeter.Infrastructure
{
    using System;
    using System.Net.Http;
    using ApiClients;
    using Application.Providers;
    using Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Polly;
    using Providers;
    using Refit;
    using Repositories;
    using SharedCore.Configuration;

    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructureModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var redisConf = configuration.GetSection<RedisCacheConfiguration>();
            services.AddStackExchangeRedisCache(options => options.Configuration = redisConf.ConnectionString);

            services.AddScoped<IPointLocationRepository, PointLocationRepository>();

            var refitSettings = new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                })
            };

            var placesApiClientConfig = configuration.GetSection<PlacesApiClientConfiguration>();
            services.AddScoped<IPlaceInfoProvider, PlaceInfoProvider>()
                .AddRefitClient<IPlacesApiClient>(refitSettings)
                .ConfigureHttpClient((_, httpClient) =>
                {
                    httpClient.BaseAddress = new Uri(placesApiClientConfig.BaseUri);
                })
                .AddTransientHttpErrorPolicy(policyBuilder =>
                    policyBuilder.WaitAndRetryAsync(placesApiClientConfig.RetryCount,
                        retryAttempt =>
                            TimeSpan.FromSeconds(Math.Pow(placesApiClientConfig.BackoffPower, retryAttempt))));

            services
                .AddHealthChecks()
                .AddRedis(redisConf.ConnectionString)
                .AddUrlGroup(new Uri(placesApiClientConfig.BaseUri), HttpMethod.Options, placesApiClientConfig.ApiName);

            return services;
        }
    }
}