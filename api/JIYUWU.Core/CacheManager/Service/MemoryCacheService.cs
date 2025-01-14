﻿using JIYUWU.Core.Common;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Caching.Memory;

namespace JIYUWU.Core.CacheManager
{
    public class MemoryCacheService : ICacheService
    {
        protected IMemoryCache _cache;
        private readonly string _prefixKey = AppSetting.GetSection("ConnectionStrs")["CacheTag"];
        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;

        }
        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            key=$"{_prefixKey}_{key}";
            return _cache.Get(key) != null;
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <returns></returns>
        public bool Add(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            key = $"{_prefixKey}_{key}";
            _cache.Set(key, value);
            return Exists(key);
        }

        public bool AddObject(string key, object value, int expireSeconds = -1, bool isSliding = false)
        {
            key = $"{_prefixKey}_{key}";
            if (expireSeconds != -1)
            {
                _cache.Set(key,
                    value,
                    new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(new TimeSpan(0, 0, expireSeconds))
                    );
            }
            else
            {
                _cache.Set(key, value);
            }

            return true;
        }
        public bool Add(string key, string value, int expireSeconds = -1, bool isSliding = false)
        {
            key = $"{_prefixKey}_{key}";
            return AddObject(key, value, expireSeconds, isSliding);
        }
        public void LPush(string key, string val)
        {
        }
        public void RPush(string key, string val)
        {
        }
        public T ListDequeue<T>(string key) where T : class
        {
            return null;
        }
        public object ListDequeue(string key)
        {
            return null;
        }
        public void ListRemove(string key, int keepIndex)
        {
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresSliding">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <param name="expiressAbsoulte">绝对过期时长</param>
        /// <returns></returns>
        public bool Add(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            key = $"{_prefixKey}_{key}";
            _cache.Set(key, value,
                    new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(expiresSliding)
                    .SetAbsoluteExpiration(expiressAbsoulte)
                    );

            return Exists(key);
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresIn">缓存时长</param>
        /// <param name="isSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <returns></returns>
        public bool Add(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            key = $"{_prefixKey}_{key}";
            if (isSliding)
                _cache.Set(key, value,
                    new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(expiresIn)
                    );
            else
                _cache.Set(key, value,
                new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(expiresIn)
                );

            return Exists(key);
        }



        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            key = $"{_prefixKey}_{key}";
            _cache.Remove(key);

            return !Exists(key);
        }
        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="key">缓存Key集合</param>
        /// <returns></returns>
        public void RemoveAll(IEnumerable<string> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            keys.ToList().ForEach(item => _cache.Remove($"{_prefixKey}_{item}"));
        }
        public string Get(string key)
        {
            key = $"{_prefixKey}_{key}";
            return _cache.Get(key)?.ToString();
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public T Get<T>(string key) where T : class
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            key = $"{_prefixKey}_{key}";
            return _cache.Get(key) as T;
        }

        public void Dispose()
        {
            if (_cache != null)
                _cache.Dispose();
            GC.SuppressFinalize(this);
        }


    }
}
