namespace CTeleport.DistanceMeter.IntegrationTests
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain.Models;
    using FluentAssertions;
    using Infrastructure.ApiClients.ApiModels;
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Newtonsoft.Json;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using WireMock.Server;
    using Xunit;
    using static SharedCore.TestConstants;
    using ApiResponseLocation = Infrastructure.ApiClients.ApiModels.Location;
    using Location = Domain.Models.Location;

    public class DistanceMeterServiceTests : IClassFixture<DistanceMeterWebAppFactory>, IDisposable
    {
        private const string CorrectUri = "/api/distance?firstiata=AMS&secondiata=BUD";
        private readonly IataPoint amsPoint = new(AmsIata, new Location(AmsLatitude, AmsLongitude));
        private readonly IataPoint budPoint = new(BudIata, new Location(BudLatitude, BudLongitude));

        private readonly Mock<IDistributedCache> cacheMock;
        private readonly HttpClient httpClient;
        private readonly WireMockServer serverMock;

        public DistanceMeterServiceTests(DistanceMeterWebAppFactory factory)
        {
            cacheMock = new Mock<IDistributedCache>();
            SetupCacheMock(amsPoint.Location);

            serverMock = ServerMock.Instance;
            httpClient = factory.CreateClient(service => service.AddSingleton(cacheMock.Object));
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        [Fact]
        public async Task GetAsync_WhenInputCorrect_ReturnsDistance()
        {
            // Arrange
            SetupServerStub();
            SetupCacheMock(amsPoint.Location);

            // Act
            var response = await httpClient.GetAsync(CorrectUri);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verify server mock received expected call
            var receivedRequest = serverMock.LogEntries.Select(x => x.RequestMessage).ToList();
            receivedRequest.Should().ContainSingle();
            receivedRequest.Select(x => x.AbsolutePath).Single().Should().Contain($"airports/{budPoint.Iata}");

            // Verify cache mock received expected call
            cacheMock.Verify(x => x.GetAsync(amsPoint.Iata, It.IsAny<CancellationToken>()), Times.Once);

            // Verify that distance calculated correctly
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<double>(jsonResponse);
            result.Should().Be(AsmBudDistance);
        }

        private void SetupCacheMock(Location location)
        {
            var cachedLocation = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(location));
            cacheMock
                .Setup(x => x.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<DistributedCacheEntryOptions>(),
                    It.IsAny<CancellationToken>()));

            cacheMock
                .Setup(x => x.GetAsync(
                    amsPoint.Iata,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedLocation);
        }

        private void SetupServerStub()
        {
            serverMock
                .Given(Request.Create().UsingGet())
                .RespondWith(Response
                    .Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithBodyAsJson(new PlacesApiResponse
                    {
                        Iata = budPoint.Iata,
                        Location = new ApiResponseLocation
                        {
                            Lat = budPoint.Location.Latitude,
                            Lon = budPoint.Location.Longitude
                        }
                    }));
        }
    }
}