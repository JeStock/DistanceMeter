namespace CTeleport.DistanceMeter.IntegrationTests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Application.Services;
    using AutoFixture;
    using Domain.Models;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Newtonsoft.Json;
    using SharedCore.Models;
    using Xunit;

    public class DistanceControllerTests : IClassFixture<DistanceMeterWebAppFactory>, IDisposable
    {
        private const string CorrectUri = "/api/distance?firstiata=ABC&secondiata=CBA";
        private const string IncorrectUri = "/api/distance?firstiata=ABC&secondiata=CBa";

        private readonly Fixture fixture;
        private readonly HttpClient httpClient;
        private readonly Mock<IDistanceMeasurementService> serviceMoq;

        public DistanceControllerTests(DistanceMeterWebAppFactory factory)
        {
            fixture = new Fixture();
            serviceMoq = new Mock<IDistanceMeasurementService>();
            httpClient = factory.CreateClient(service => service.AddSingleton(serviceMoq.Object));
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        [Fact]
        public async Task GetAsync_WhenInputCorrect_ReturnsDistance()
        {
            // Arrange
            var expectedDistance = fixture.Create<double>();
            SetupServiceGetDistanceAsyncMethod(new Result<double>(expectedDistance));

            // Act
            var response = await httpClient.GetAsync(CorrectUri);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<double>(jsonResponse);
            result.Should().Be(expectedDistance);
        }

        [Fact]
        public async Task GetAsync_WhenIncorrectIataCodeProvided_ReturnsBadRequest()
        {
            // Arrange
            var expectedDistance = fixture.Create<double>();
            SetupServiceGetDistanceAsyncMethod(new Result<double>(expectedDistance));

            // Act
            var response = await httpClient.GetAsync(IncorrectUri);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private void SetupServiceGetDistanceAsyncMethod(Result<double> result)
        {
            serviceMoq
                .Setup(x => x.GetDistanceAsync(
                    It.IsAny<IataCouple>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
        }
    }
}