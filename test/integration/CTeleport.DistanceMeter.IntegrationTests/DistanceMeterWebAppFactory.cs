namespace CTeleport.DistanceMeter.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Api;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class DistanceMeterWebAppFactory : WebApplicationFactory<Startup>
    {
        public HttpClient CreateClient(Action<IServiceCollection> registerServices)
        {
            return WithWebHostBuilder(configuration =>
            {
                configuration.ConfigureTestServices(registerServices);
                configuration.ConfigureAppConfiguration((_, builder) =>
                {
                    builder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["PlacesApiClientConfiguration:BaseUri"] = $"{ServerMock.Url}/airports"
                    });
                });
            }).CreateClient();
        }
    }
}