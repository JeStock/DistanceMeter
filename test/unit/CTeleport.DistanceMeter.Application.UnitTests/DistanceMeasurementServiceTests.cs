namespace CTeleport.DistanceMeter.Application.UnitTests
{
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Domain.Models;
    using FluentAssertions;
    using Moq;
    using Providers;
    using Services;
    using SharedCore.Models;
    using Xunit;
    using static SharedCore.TestConstants;

    public class DistanceMeasurementServiceTests
    {
        private readonly Fixture fixture;
        private readonly Mock<IPlaceInfoProvider> placeInfoProviderMock;

        public DistanceMeasurementServiceTests()
        {
            fixture = new Fixture();
            placeInfoProviderMock = new Mock<IPlaceInfoProvider>();
        }

        [Fact]
        public async Task GetDistanceAsync_WhenInputCorrect_ReturnsDistanceResult()
        {
            // Arrange
            var couple = new IataCouple
            {
                FirstIata = AmsIata,
                SecondIata = BudIata
            };

            var service = new DistanceMeasurementService(placeInfoProviderMock.Object);

            SetupPlaceInfoProviderGetPlaceInfoAsyncMethod(
                AmsIata,
                new Result<IataPoint>(new IataPoint(AmsIata, new Location(AmsLatitude, AmsLongitude))));

            SetupPlaceInfoProviderGetPlaceInfoAsyncMethod(
                BudIata,
                new Result<IataPoint>(new IataPoint(BudIata, new Location(BudLatitude, BudLongitude))));

            // Act
            var serviceResponse = await service.GetDistanceAsync(couple, CancellationToken.None);

            // Assert
            serviceResponse.IsError.Should().BeFalse();
            serviceResponse.Data.Should().Be(AsmBudDistance);
        }

        [Fact]
        public async Task GetDistanceAsync_WhenOneOfIataCodesIsIncorrect_ReturnsErrorResult()
        {
            // Arrange
            var incorrectIata = fixture.Create<string>();
            var couple = new IataCouple
            {
                FirstIata = AmsIata,
                SecondIata = incorrectIata
            };

            var service = new DistanceMeasurementService(placeInfoProviderMock.Object);

            SetupPlaceInfoProviderGetPlaceInfoAsyncMethod(
                AmsIata,
                new Result<IataPoint>(new IataPoint(AmsIata, new Location(AmsLatitude, AmsLongitude))));

            SetupPlaceInfoProviderGetPlaceInfoAsyncMethod(incorrectIata,
                new Result<IataPoint>(new Error(fixture.Create<string>())));

            // Act
            var serviceResponse = await service.GetDistanceAsync(couple, CancellationToken.None);

            // Assert
            serviceResponse.IsError.Should().BeTrue();
        }

        private void SetupPlaceInfoProviderGetPlaceInfoAsyncMethod(string iata, Result<IataPoint> result)
        {
            placeInfoProviderMock
                .Setup(x => x.GetPlaceInfoAsync(
                    iata,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
        }
    }
}