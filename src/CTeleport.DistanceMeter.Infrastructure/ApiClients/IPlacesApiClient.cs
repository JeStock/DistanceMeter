namespace CTeleport.DistanceMeter.Infrastructure.ApiClients
{
    using System.Threading;
    using System.Threading.Tasks;
    using ApiModels;
    using Refit;

    public interface IPlacesApiClient
    {
        [Get("/{iataCode}")]
        [Headers("Accept: application/json")]
        Task<ApiResponse<PlacesApiResponse>> GetPlaceInfoAsync(string iataCode, CancellationToken token);
    }
}