namespace CTeleport.DistanceMeter.Infrastructure.UnitTests
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using ApiClients;
    using ApiClients.ApiModels;
    using AutoFixture;
    using Domain.Models;
    using FluentAssertions;
    using Moq;
    using Providers;
    using Refit;
    using Repositories;
    using SharedCore.Models;
    using Xunit;
    using Location = Domain.Models.Location;

    public class PlaceInfoProviderTests
    {
        private readonly PlacesApiResponse placeInfo;
        private readonly Mock<IPlacesApiClient> placesApiClientMock;
        private readonly Mock<IPointLocationRepository> repositoryMock;

        public PlaceInfoProviderTests()
        {
            var fixture = new Fixture();
            placeInfo = fixture.Create<PlacesApiResponse>();
            placesApiClientMock = new Mock<IPlacesApiClient>();
            repositoryMock = new Mock<IPointLocationRepository>();
        }

        [Fact]
        public async Task GetPlaceInfoAsync_WhenPlaceLocationCached_ReturnsIataPointFromCache()
        {
            // Arrange
            var location = new Location(placeInfo.Location.Lat, placeInfo.Location.Lon);
            var expected = new IataPoint(placeInfo.Iata, location);
            var provider = new PlaceInfoProvider(placesApiClientMock.Object, repositoryMock.Object);

            SetupRepositoryGetAsyncMethod(location);

            // Act
            var result = await provider.GetPlaceInfoAsync(placeInfo.Iata, CancellationToken.None);

            // Assert
            result.Should().BeOfType<Result<IataPoint>>();
            result.Data.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetPlaceInfoAsync_WhenApiClientSucceed_ReturnsIataPointFromApi()
        {
            // Arrange
            var location = new Location(placeInfo.Location.Lat, placeInfo.Location.Lon);
            var expected = new IataPoint(placeInfo.Iata, location);
            var apiResponse = new ApiResponse<PlacesApiResponse>(new HttpResponseMessage(HttpStatusCode.OK), placeInfo);
            var provider = new PlaceInfoProvider(placesApiClientMock.Object, repositoryMock.Object);

            SetupApiClientGetPlaceInfoAsyncMethod(apiResponse);

            // Act
            var result = await provider.GetPlaceInfoAsync(placeInfo.Iata, CancellationToken.None);

            // Assert
            result.Should().BeOfType<Result<IataPoint>>();
            result.Data.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetPlaceInfoAsync_WhenApiClientFails_ReturnsErrorResult()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
            var exception = await ApiException.Create(new HttpRequestMessage(), HttpMethod.Get, responseMessage);
            var apiResponse = new ApiResponse<PlacesApiResponse>(responseMessage, null, exception);
            var provider = new PlaceInfoProvider(placesApiClientMock.Object, repositoryMock.Object);

            SetupApiClientGetPlaceInfoAsyncMethod(apiResponse);

            // Act
            var result = await provider.GetPlaceInfoAsync(placeInfo.Iata, CancellationToken.None);

            // Assert
            result.Should().BeOfType<Result<IataPoint>>();
            result.IsError.Should().BeTrue();
        }

        private void SetupRepositoryGetAsyncMethod(Location location)
        {
            repositoryMock
                .Setup(x => x.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(location);
        }

        private void SetupApiClientGetPlaceInfoAsyncMethod(ApiResponse<PlacesApiResponse> apiResponse)
        {
            placesApiClientMock
                .Setup(x => x.GetPlaceInfoAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);
        }
    }
}