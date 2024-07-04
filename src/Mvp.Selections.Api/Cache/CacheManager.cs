using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Mvp.Selections.Api.Extensions;

namespace Mvp.Selections.Api.Cache
{
    public class CacheManager(IOptions<Configuration.CacheOptions> options, IMemoryCache cache)
        : ICacheManager
    {
        private readonly Dictionary<CacheCollection, CacheInvalidationToken> _tokens =
            new()
            {
                { CacheCollection.MvpProfileSearchResults, new CacheInvalidationToken() }
            };

        private readonly Dictionary<CacheCollection, TimeSpan> _expirations =
            new()
            {
                { CacheCollection.MvpProfileSearchResults, TimeSpan.FromSeconds(options.Value.MvpProfilesCacheDurationInSeconds) }
            };

        public enum CacheCollection
        {
            MvpProfileSearchResults
        }

        public bool TryGet<T>(string key, out T? value)
        {
            return cache.TryGetValue(key, out value);
        }

        public T Set<T>(CacheCollection collection, string key, T value)
        {
            MemoryCacheEntryOptions entryOptions =
                new MemoryCacheEntryOptions().AddExpirationToken(_tokens[collection]);
            if (_expirations.TryGetValue(collection, out TimeSpan expiration))
            {
                entryOptions.SetAbsoluteExpiration(expiration);
            }

            return cache.Set(key, value, entryOptions);
        }

        public void Clear(CacheCollection collection)
        {
            _tokens[collection].Cancel();
            _tokens[collection].Reset();
        }

        public string GetMvpProfileSearchResultsKey(
            string? text = null,
            IList<short>? mvpTypeIds = null,
            IList<short>? years = null,
            IList<short>? countryIds = null,
            int page = 1,
            short pageSize = 100)
        {
            return
                $"{options.Value.MvpProfilesCacheKey}_{text}_{mvpTypeIds.ToCommaSeparatedStringOrNullLiteral()}_{years.ToCommaSeparatedStringOrNullLiteral()}_{countryIds.ToCommaSeparatedStringOrNullLiteral()}";
        }

        private class CacheInvalidationToken
            : IChangeToken
        {
            private CancellationTokenSource _cts = new();

            public bool HasChanged => _cts.Token.IsCancellationRequested;

            public bool ActiveChangeCallbacks => true;

            public IDisposable RegisterChangeCallback(Action<object?> callback, object? state)
            {
                return _cts.Token.Register(callback, state);
            }

            public void Cancel()
            {
                _cts.Cancel();
            }

            public void Reset()
            {
                _cts = new CancellationTokenSource();
            }
        }
    }
}
