namespace CTeleport.DistanceMeter.Infrastructure.UnitTests
{
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Domain.Models;
    using FluentAssertions;
    using Microsoft.Extensions.Caching.Distributed;
    using Moq;
    using Newtonsoft.Json;
    using Repositories;
    using Xunit;

    public class PointLocationRepositoryTests
    {
        private readonly Mock<IDistributedCache> cacheMock;
        private readonly Fixture fixture;

        public PointLocationRepositoryTests()
        {
            fixture = new Fixture();
            cacheMock = new Mock<IDistributedCache>();
        }

        [Fact]
        public async Task TryGetDistanceAsync_WhenValueCached_ReturnsResult()
        {
            // Arrange
            var iata = fixture.Create<string>();
            var expectedValue = fixture.Create<Location>();
            var expectedValueEncoded = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(expectedValue));
            var repository = new PointLocationRepository(cacheMock.Object);

            SetupCacheGetAsyncMethod(expectedValueEncoded);

            // Act
            var distance = await repository.GetAsync(iata, CancellationToken.None);

            // Assert
            distance.Should().BeEquivalentTo(expectedValue);
        }

        [Fact]
        public async Task TryGetDistanceAsync_WhenValueNotCached_ReturnsNull()
        {
            // Arrange
            var iata = fixture.Create<string>();
            var repository = new PointLocationRepository(cacheMock.Object);

            SetupCacheGetAsyncMethod(null);

            // Act
            var distance = await repository.GetAsync(iata, CancellationToken.None);

            // Assert
            distance.Should().BeNull();
        }

        [Fact]
        public async Task StoreDistanceAsync_WhenValueProvided_StoreValueByExpectedKey()
        {
            // Arrange
            var iata = fixture.Create<string>();
            var repository = new PointLocationRepository(cacheMock.Object);

            SetupCacheSaveAsyncMethod();

            // Act
            await repository.SetAsync(iata, fixture.Create<Location>(), CancellationToken.None);

            // Arrange
            cacheMock
                .Verify(x => x.SetAsync(
                    iata,
                    It.IsAny<byte[]>(),
                    It.IsAny<DistributedCacheEntryOptions>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        private void SetupCacheGetAsyncMethod(byte[] expectedResult)
        {
            cacheMock
                .Setup(x => x.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);
        }

        private void SetupCacheSaveAsyncMethod()
        {
            cacheMock
                .Setup(x => x.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<DistributedCacheEntryOptions>(),
                    It.IsAny<CancellationToken>()));
        }
    }
}