namespace CTeleport.DistanceMeter.Infrastructure.Repositories
{
    using Domain.Models;
    using Microsoft.Extensions.Caching.Distributed;

    public class PointLocationRepository : Repository<Location>, IPointLocationRepository
    {
        public PointLocationRepository(IDistributedCache cache) : base(cache) { }
    }
}