namespace CTeleport.DistanceMeter.Infrastructure.Repositories
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRepository<T> where T : class
    {
        Task<T> GetAsync(string key, CancellationToken token);
        Task SetAsync(string key, T value, CancellationToken token);
    }
}