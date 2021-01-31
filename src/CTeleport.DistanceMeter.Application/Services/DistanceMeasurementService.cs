namespace CTeleport.DistanceMeter.Application.Services
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Constants;
    using Domain.Models;
    using Providers;
    using SharedCore.Models;

    public class DistanceMeasurementService : IDistanceMeasurementService
    {
        private readonly IPlaceInfoProvider placeInfoProvider;

        public DistanceMeasurementService(IPlaceInfoProvider placeInfoProvider)
        {
            this.placeInfoProvider = placeInfoProvider;
        }

        public async Task<Result<double>> GetDistanceAsync(IataCouple couple, CancellationToken token)
        {
            var firstPlace = placeInfoProvider.GetPlaceInfoAsync(couple.FirstIata, token);
            var secondSecond = placeInfoProvider.GetPlaceInfoAsync(couple.SecondIata, token);
            var results = await Task.WhenAll(firstPlace, secondSecond);

            if (results.Any(x => x.IsError))
            {
                var errors = results
                    .Where(x => x.IsError)
                    .Aggregate(string.Empty, (current, next) => $"{current} {next.Error.Message}");

                var message = string.Format(ErrorMessages.ExternalApiErrorMessage, errors);
                return new Result<double>(new Error(message));
            }

            var firstPlaceInfo = results.First(x => x.Data.Iata == couple.FirstIata).Data;
            var secondPlaceInfo = results.First(x => x.Data.Iata == couple.SecondIata).Data;

            return new Result<double>(firstPlaceInfo.DistanceTo(secondPlaceInfo));
        }
    }
}