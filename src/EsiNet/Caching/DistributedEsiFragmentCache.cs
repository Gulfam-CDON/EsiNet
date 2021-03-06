﻿using System;
using System.Threading.Tasks;
using EsiNet.Caching.Serialization;
using Microsoft.Extensions.Caching.Distributed;

namespace EsiNet.Caching
{
    public class DistributedEsiFragmentCache : IEsiFragmentCache
    {
        private readonly IDistributedCache _cache;
        private readonly ISerializer _serializer;

        public DistributedEsiFragmentCache(IDistributedCache cache, ISerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<(bool, T)> TryGet<T>(CacheKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var cachedBytes = await _cache.GetAsync(CreateFullKey<T>(key));
            if (cachedBytes != null)
            {
                var envelope = _serializer.DeserializeBytes<CacheEnvelope<T>>(cachedBytes);
                return (true, envelope.Body);
            }

            return (false, default(T));
        }

        public async Task Set<T>(CacheKey key, T value, TimeSpan absoluteExpirationRelativeToNow)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
            };
            var envelope = new CacheEnvelope<T>(value, absoluteExpirationRelativeToNow);
            var bytes = _serializer.SerializeBytes(envelope);
            await _cache.SetAsync(CreateFullKey<T>(key), bytes, options);
        }

        private static string CreateFullKey<T>(CacheKey key)
        {
            return $"Esi_{typeof(T).Name}_{key}";
        }
    }
}