namespace CTeleport.DistanceMeter.Infrastructure.Repositories
{
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Distributed;
    using Newtonsoft.Json;

    public abstract class Repository<T> : IRepository<T> where T : class
    {
        private readonly IDistributedCache cache;

        protected Repository(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public async Task<T> GetAsync(string key, CancellationToken token)
        {
            var encodedValue = await cache.GetAsync(key, token);
            if (encodedValue != null)
            {
                var serializedValue = Encoding.UTF8.GetString(encodedValue);
                var value = JsonConvert.DeserializeObject<T>(serializedValue);

                return value;
            }

            return null;
        }

        public async Task SetAsync(string key, T value, CancellationToken token)
        {
            var serializedValue = JsonConvert.SerializeObject(value);
            var encodedValue = Encoding.UTF8.GetBytes(serializedValue);

            await cache.SetAsync(key, encodedValue, token);
        }
    }
}