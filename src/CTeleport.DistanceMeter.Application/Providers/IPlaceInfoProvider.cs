namespace CTeleport.DistanceMeter.Application.Providers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Domain.Models;
    using SharedCore.Models;

    public interface IPlaceInfoProvider
    {
        Task<Result<IataPoint>> GetPlaceInfoAsync(string iata, CancellationToken token);
    }
}