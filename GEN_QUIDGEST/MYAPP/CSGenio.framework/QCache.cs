using System;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;

namespace CSGenio.framework
{
    /// <summary>
    /// Represents an in memory cache singleton. This class is thread safe.
    /// </summary>
    public sealed class QCache
    {
        /*
        *   Beware that changes made to these params and fiels 
        *   WILL impact webadmin!
        */
        
        /// <summary>
        /// General-purpose instance for storing database records.
        /// </summary>
        /// <remarks>
        /// To prevent collisions, keys should include the table prefix.
        /// </remarks>
        public QCacheInstance Records { get; private set; }

        /// <summary>
        /// Instance for storing user-related data.
        /// </summary>
        public QCacheInstance User { get; private set; }

        /// <summary>
        /// Instance for managing file upload tickets.
        /// </summary>
        public QCacheInstance FileUpload { get; private set; }

        /// <summary>
        /// Instance for caching files to export.
        /// </summary>
        public QCacheInstance ExportFiles { get; private set; }

        /// <summary>
        /// Instance for caching widget data.
        /// </summary>
        public QCacheInstance Dashboard { get; private set; }
        
        /// <summary>
        /// Instance for caching array class types information.
        /// </summary>
        public QCacheInstance Array { get; private set; }

        /// <summary>
        /// Instance for storing frequently accessed information during reindexation.
        /// </summary>
        public QCacheInstance AdminReindexation { get; private set; }

        /// <summary>
        /// General-purpose instance for usage in manual code.
        /// </summary>
        /// <remarks>
        /// To prevent collisions, keys should include the appropriate prefixes.
        /// </remarks>
        public QCacheInstance ManualCode { get; private set; }

        private static readonly QCache m_instance = new QCache();

        /// <summary>
        /// Get the cache singleton
        /// </summary>
        public static QCache Instance { get { return m_instance; } }

        private QCache()
        {
            Records = new QCacheInstance(defaultEntryExpiration: TimeSpan.FromSeconds(5));
            User = new QCacheInstance();
            FileUpload = new QCacheInstance();
            ExportFiles = new QCacheInstance(defaultEntryExpiration: TimeSpan.FromMinutes(1));
            Dashboard = new QCacheInstance();
            Array = new QCacheInstance();
            AdminReindexation = new QCacheInstance();
            ManualCode = new QCacheInstance();
        }
    }

    public sealed class QCacheInstance
    {
        public IMemoryCache Instance { get; private set; }
        public MemoryCacheOptions CacheOptions { get; private set; }
        public TimeSpan DefaultEntryExpiration { get; private set; }

        private readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        public QCacheInstance(MemoryCacheOptions options = null, TimeSpan? defaultEntryExpiration = null)
        {
            if (options == null) options = new MemoryCacheOptions();

            Instance = new MemoryCache(options);
            DefaultEntryExpiration = defaultEntryExpiration ?? TimeSpan.FromMinutes(30);
        }

        /// <summary>
        /// Retrieves an item from the cache given its key
        /// </summary>
        /// <param name="key">The key of the item to retreive</param>
        /// <returns>The item value or null in case the item is not found or has been invalidated</returns>
        public object Get(string key)
        {
            cacheLock.EnterReadLock();
            try
            {
                return Instance.Get(key);
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Commits a value to cache with the default entry expiration policy.
        /// If it already exists it will be updated.
        /// </summary>
        /// <param name="key">The key of the item</param>
        /// <param name="item">The value to add to the cache</param>
        public void Put(string key, object item)
        {
            this.Put(key, item, DefaultEntryExpiration);
        }

        /// <summary>
        /// Commits a value to cache. If it already exists it will be updated.
        /// </summary>
        /// <param name="key">The key of the item</param>
        /// <param name="item">The value to add to the cache</param>
        /// <param name="timeToLive">The amount of time the item will be cached for</param>
        public void Put(string key, object item, TimeSpan timeToLive)
        {
            MemoryCacheEntryOptions cacheEntryOptions =
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.Add(timeToLive)
                };

            this.Put(key, item, cacheEntryOptions);
        }

        /// <summary>
        /// Commits a value to cache. If it already exists it will be updated.
        /// </summary>
        /// <param name="key">The key of the item</param>
        /// <param name="item">The item to add to the cache</param>
        /// <param name="options">Optional cache options specific to the item being added</param>
        public void Put(string key, object item, MemoryCacheEntryOptions options)
        {
            cacheLock.EnterWriteLock();
            try
            {
                Instance.Set(key, item, options);
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Explicitly invalidates an item
        /// </summary>
        /// <param name="key">The key of the item to invalidate</param>
        public void Invalidate(string key)
        {
            cacheLock.EnterWriteLock();
            try
            {
                Instance.Remove(key);
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        ~QCacheInstance()
        {
            cacheLock?.Dispose();
        }
    }
}
