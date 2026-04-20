using System;
using System.Collections.Generic;
using System.Text;

namespace CSGenio.framework
{

    /// <summary>
    /// Cache policy to invalidate items
    /// </summary>
    public class ClassPolicy
    {
        /// <summary>
        /// An absolute time at wich the item will be invalid
        /// </summary>
        public DateTime AbsoluteExpiration { get; set; }
        /// <summary>
        /// A span of time after item creation where the item will become invalid
        /// </summary>
        public TimeSpan SlidingExpiration { get; set; }
        /// <summary>
        /// A priority so that higher priority items will live longer during memory shortage
        /// </summary>
        public int Priority { get; set; }
    }

    /// <summary>
    /// Stores the cached item value toghether with its invalidation information
    /// </summary>
    public class CacheItem
    {
        /// <summary>
        /// The cached value
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// Time of creation
        /// </summary>
        public DateTime TimeStamp { get; set; }
        /// <summary>
        /// Invalidation policy
        /// </summary>
        public ClassPolicy CachePolicy { get; set; }
    }

    /// <summary>
    /// Represents an in memory cache singleton. This class is thread safe.
    /// </summary>
    [Obsolete("Use QCache instead")]
    public class Cache
    {
        private Dictionary<string, CacheItem> m_cache = new Dictionary<string, CacheItem>();
        private object cacheLock = new object();

        private static readonly Cache m_instance = new Cache();

        private Cache() 
        { 
            DefaultPolicy = new ClassPolicy(); 
        }

        /// <summary>
        /// Get the cache singleton
        /// </summary>
        public static Cache Instance { get { return m_instance; } }

        /// <summary>
        /// Maximum number of items in the cache
        /// </summary>
        public int MaxItems { get; set; }
        /// <summary>
        /// Get os sets the default policy applied to new cache items
        /// </summary>
        public ClassPolicy DefaultPolicy { get; set; }

        /// <summary>
        /// Commits a value to cache. If it already exists it will be updated.
        /// </summary>
        /// <param name="key">The key of the item</param>
        /// <param name="value">The value to add to the cache</param>
        public void Put(string key, object value)
        {
            lock (cacheLock)
            {
                //TODO: check if we need to drop items because we are above the max items

                CacheItem ci = null;
                m_cache.TryGetValue(key, out ci);
                if (ci == null)
                {
                    ci = new CacheItem();
                    ci.TimeStamp = DateTime.Now;
                    ci.Value = value;
                    ci.CachePolicy = DefaultPolicy;
                }
                else
                {
                    ci.TimeStamp = DateTime.Now;
                }

                m_cache[key] = ci;
            }
        }

        /// <summary>
        /// Retreives an item from the cache given its key
        /// </summary>
        /// <param name="key">The key of the item to retreive</param>
        /// <returns>The item value or null in case the item is not found or has been invalidated</returns>
        public object Get(string key)
        {
            lock (cacheLock)
            {
                CacheItem res = null;
                m_cache.TryGetValue(key, out res);
                if (res == null)
                    return null;
                else if( CheckInvalid(res) )
                    return null;
                else                
                    return res.Value;
            }
        }

        private bool CheckInvalid(CacheItem res)
        {
            if (res.CachePolicy.SlidingExpiration != TimeSpan.Zero && res.TimeStamp + res.CachePolicy.SlidingExpiration < DateTime.Now)
                return true;
            
            return false;
        }
       
        /// <summary>
        /// Explicitly invalidates an item
        /// </summary>
        /// <param name="key">The key of the item to invalidate</param>
        public void Invalidate(string key)
        {
            lock (cacheLock)
            {
                m_cache.Remove(key);
            }
        }
    }
}
