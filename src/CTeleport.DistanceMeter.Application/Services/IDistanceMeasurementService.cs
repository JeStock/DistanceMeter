namespace CTeleport.DistanceMeter.Application.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using Domain.Models;
    using SharedCore.Models;

    public interface IDistanceMeasurementService
    {
        Task<Result<double>> GetDistanceAsync(IataCouple couple, CancellationToken token);
    }
}