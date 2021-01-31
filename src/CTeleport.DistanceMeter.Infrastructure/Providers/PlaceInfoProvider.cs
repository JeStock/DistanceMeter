namespace CTeleport.DistanceMeter.Infrastructure.Providers
{
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using ApiClients;
    using Application.Constants;
    using Application.Providers;
    using Domain.Models;
    using Repositories;
    using SharedCore.Models;

    public class PlaceInfoProvider : IPlaceInfoProvider
    {
        private readonly IPlacesApiClient apiClient;
        private readonly IPointLocationRepository repository;

        public PlaceInfoProvider(IPlacesApiClient apiClient, IPointLocationRepository repository)
        {
            this.apiClient = apiClient;
            this.repository = repository;
        }

        public async Task<Result<IataPoint>> GetPlaceInfoAsync(string iata, CancellationToken token)
        {
            var cachedLocation = await repository.GetAsync(iata, token);
            if (cachedLocation != null)
            {
                return new Result<IataPoint>(new IataPoint(iata, cachedLocation));
            }

            var response = await apiClient.GetPlaceInfoAsync(iata, token);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    var message = string.Format(ErrorMessages.ExternalApiIataNotFound, iata);
                    return new Result<IataPoint>(new Error(message));
                }

                return new Result<IataPoint>(new Error(ErrorMessages.ExternalApiFailed));
            }

            var placeInfo = response.Content;
            var location = new Location(placeInfo.Location.Lat, placeInfo.Location.Lon);
            var iataPoint = new IataPoint(placeInfo.Iata, location);

            await repository.SetAsync(iata, location, token);

            return new Result<IataPoint>(iataPoint);
        }
    }
}