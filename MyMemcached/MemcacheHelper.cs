using Memcached.ClientLibrary;
using MyMemcached.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMemcached
{
    /// <summary>
    /// Memcache通用类
    /// </summary>
    public class MemcacheHelper
    {
        /// <summary>
        /// 默认Memcache客户端的代理类对象
        /// </summary>
        public static MemcachedClient mc = null;
        public static ArrayList serverList = null;
        static MemcacheHelper()
        {
            CreateServer();
        }
        #region 创建Memcache服务
        /// <summary>
        /// 创建Memcache服务
        /// </summary>
        /// <param name="serverlist">IP端口列表</param>
        /// <param name="poolName">Socket连接池名称</param>
        /// <returns>Memcache客户端代理类</returns>
        private static void CreateServer()
        {
            try
            {
                serverList = new ArrayList();
                //获取Memcached服务配置
                ServersConfigInfo config = ServersConfig.GetConfig();
                foreach (Server s in config.Servers)
                {
                    string host = s.IP + ":" + s.Port;
                    //初始化Memcache服务器池
                    SockIOPool pool = SockIOPool.GetInstance(s.PoolName);

                    //设置Memcache池连接点服务器端。
                    pool.SetServers(new string[] { host });
                    pool.InitConnections = config.InitConnections;//初始连接数                
                    pool.MinConnections = config.MinConnections;//最小连接数                
                    pool.MaxConnections = config.MaxConnections;//最大连接数
                    pool.SocketConnectTimeout = config.SocketConnectTimeout;//设置连接的套接字超时  
                    pool.SocketTimeout = config.SocketTimeout;//设置套接字超时读取  
                    pool.MaintenanceSleep = config.MaintenanceSleep;//设置维护线程运行的睡眠时间。如果设置为0，那么维护线程将不会启动,30就是每隔30秒醒来一次  
                    //获取或设置池的故障标志。  
                    //如果这个标志被设置为true则socket连接失败，将试图从另一台服务器返回一个套接字如果存在的话。  
                    //如果设置为false，则得到一个套接字如果存在的话。否则返回NULL，如果它无法连接到请求的服务器。  
                    pool.Failover = config.Failover;
                    pool.Nagle = config.Nagle;//如果为false，对所有创建的套接字关闭Nagle的算法  
                    pool.Initialize();

                    serverList.Add(host);
                }
                //创建默认Memcache客户端实例。
                mc = GetClient(config.DefaultServer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取客户端
        public static MemcachedClient GetClient(string server)
        {
            MemcachedClient current = Singleton<MemcachedClient>.Instance;
            current.PoolName = server;
            current.EnableCompression = false;//是否压缩
            return current;
        }
        #endregion

        #region 缓存是否存在
        /// <summary>
        /// 缓存是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static bool CacheIsExists(string key)
        {
            if (mc.KeyExists(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 默认服务器

        #region 写(Set)
        /// <summary>
        /// 设置数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void Set(string key, object value)
        {
            mc.Set(key, value);
        }
        /// <summary>
        /// 设置数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="hashCode">哈希码</param>
        public static void Set(string key, object value, int hashCode)
        {
            mc.Set(key, value, hashCode);
        }
        /// <summary>
        /// 设置数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        public static void Set(string key, object value, DateTime expiry)
        {
            mc.Set(key, value, expiry);
        }
        /// <summary>
        /// 设置数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        public static void Set(string key, object value, DateTime expiry, int hashCode)
        {
            mc.Set(key, value, expiry, hashCode);
        }
        #endregion

        #region 读(Get)

        #region 返回泛型
        /// <summary>
        /// 读取数据缓存
        /// </summary>
        /// <param name="key">键</param>
        public static T Get<T>(string key)
        {
            return (T)mc.Get(key);
        }
        /// <summary>
        /// 读取数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashCode">哈希码</param>
        public static T Get<T>(string key, int hashCode)
        {
            return (T)mc.Get(key, hashCode);
        }
        /// <summary>
        /// 读取数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="asString">是否把值作为字符串返回</param>
        public static T Get<T>(string key, object value, bool asString)
        {
            return (T)mc.Get(key, value, asString);
        }
        #endregion

        /// <summary>
        /// 读取数据缓存
        /// </summary>
        /// <param name="key">键</param>
        public static object Get(string key)
        {
            return mc.Get(key);
        }
        /// <summary>
        /// 读取数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashCode">哈希码</param>
        public static object Get(string key, int hashCode)
        {
            return mc.Get(key, hashCode);
        }
        /// <summary>
        /// 读取数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="asString">是否把值作为字符串返回</param>
        public static object Get(string key, object value, bool asString)
        {
            return mc.Get(key, value, asString);
        }
        #endregion

        #region 批量写(Set)
        /// <summary>
        /// 批量设置数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetMultiple(string[] keys, object[] values)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                mc.Set(keys[i], values[i]);
            }
        }
        /// <summary>
        /// 批量设置数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="hashCode">哈希码</param>
        public static void SetMultiple(string[] keys, object[] values, int[] hashCodes)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                mc.Set(keys[i], values[i], hashCodes[i]);
            }
        }
        /// <summary>
        /// 批量设置数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        public static void SetMultiple(string[] keys, object[] values, DateTime[] expirys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                mc.Set(keys[i], values[i], expirys[i]);
            }
        }
        /// <summary>
        /// 批量设置数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        public static void Set(string[] keys, object[] values, DateTime[] expirys, int[] hashCodes)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                mc.Set(keys[i], values[i], expirys[i], hashCodes[i]);
            }
        }
        #endregion

        #region 批量读取(Multiple),返回哈希表 Hashtable
        /// <summary>
        /// 批量读取数据缓存
        /// </summary>
        /// <param name="keys">键集合</param>
        public static Hashtable GetMultiple(string[] keys)
        {
            return mc.GetMultiple(keys);
        }
        /// <summary>
        /// 批量读取数据缓存
        /// </summary>
        /// <param name="keys">键集合</param>
        /// <param name="hashCodes">哈希码集合</param>
        public static Hashtable GetMultiple(string[] keys, int[] hashCodes)
        {
            return mc.GetMultiple(keys, hashCodes);
        }
        /// <summary>
        /// 批量读取数据缓存
        /// </summary>
        /// <param name="keys">键集合</param>
        /// <param name="hashCodes">哈希码集合</param>
        /// <param name="asString">所有值返回字符</param>
        public static Hashtable GetMultiple(string[] keys, int[] hashCodes, bool asString)
        {
            return mc.GetMultiple(keys, hashCodes, asString);
        }
        #endregion

        #region 批量读取(Multiple),返回对象数组object[]
        /// <summary>
        /// 批量读取数据缓存
        /// </summary>
        /// <param name="keys">键集合</param>
        public static object[] GetMultipleArray(string[] keys)
        {
            return mc.GetMultipleArray(keys);
        }
        /// <summary>
        /// 批量读取数据缓存
        /// </summary>
        /// <param name="keys">键集合</param>
        /// <param name="hashCodes">哈希码集合</param>
        public static object[] GetMultipleArray(string[] keys, int[] hashCodes)
        {
            return mc.GetMultipleArray(keys, hashCodes);
        }
        /// <summary>
        /// 批量读取数据缓存
        /// </summary>
        /// <param name="keys">键集合</param>
        /// <param name="hashCodes">哈希码集合</param>
        /// <param name="asString">所有值返回字符</param>
        public static object[] GetMultipleArray(string[] keys, int[] hashCodes, bool asString)
        {
            return mc.GetMultipleArray(keys, hashCodes, asString);
        }
        #endregion

        #region 批量读取(Multiple),返回泛型集合List[T]
        /// <summary>
        /// 批量读取数据缓存
        /// </summary>
        /// <param name="keys">键集合</param>
        public static List<T> GetMultipleList<T>(string[] keys)
        {
            object[] obj = mc.GetMultipleArray(keys);
            List<T> list = new List<T>();
            foreach (object o in obj)
            {
                list.Add((T)o);
            }
            return list;
        }
        /// <summary>
        /// 批量读取数据缓存
        /// </summary>
        /// <param name="keys">键集合</param>
        /// <param name="hashCodes">哈希码集合</param>
        public static List<T> GetMultipleList<T>(string[] keys, int[] hashCodes)
        {
            object[] obj = mc.GetMultipleArray(keys, hashCodes);
            List<T> list = new List<T>();
            foreach (object o in obj)
            {
                list.Add((T)o);
            }
            return list;
        }
        /// <summary>
        /// 批量读取数据缓存
        /// </summary>
        /// <param name="keys">键集合</param>
        /// <param name="hashCodes">哈希码集合</param>
        /// <param name="asString">所有值返回字符</param>
        public static List<T> GetMultipleList<T>(string[] keys, int[] hashCodes, bool asString)
        {
            object[] obj = mc.GetMultipleArray(keys, hashCodes, asString);
            List<T> list = new List<T>();
            foreach (object o in obj)
            {
                list.Add((T)o);
            }
            return list;
        }
        #endregion

        #region 替换更新(Replace)
        /// <summary>
        /// 替换更新数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void Replace(string key, object value)
        {
            mc.Replace(key, value);
        }
        /// <summary>
        /// 替换更新数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="hashCode">哈希码</param>
        public static void Replace(string key, object value, int hashCode)
        {
            mc.Replace(key, value, hashCode);
        }
        /// <summary>
        /// 替换更新数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        public static void Replace(string key, object value, DateTime expiry)
        {
            mc.Replace(key, value, expiry);
        }
        /// <summary>
        /// 替换更新数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        public static void Replace(string key, object value, DateTime expiry, int hashCode)
        {
            mc.Replace(key, value, expiry, hashCode);
        }
        #endregion

        #region 删除(Delete)

        /// <summary>
        ///删除指定条件缓存
        /// </summary>
        /// <param name="key">键</param>
        public static bool Delete(string key)
        {
            return mc.Delete(key);
        }
        /// <summary>
        /// 删除指定条件缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashCode">哈希码</param>
        /// <param name="expiry">过期时间</param>
        public static bool Delete(string key, int hashCode, DateTime expiry)
        {
            return mc.Delete(key, hashCode, expiry);
        }
        /// <summary>
        /// 删除指定条件缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="expiry">过期时间</param>
        public static bool Delete(string key, DateTime expiry)
        {
            return mc.Delete(key, expiry);
        }

        /// <summary>
        /// 移除全部缓存
        /// </summary>
        public static void RemovAllCache()
        {
            mc.FlushAll();
        }
        /// <summary>
        /// 移除全部缓存
        /// </summary>
        /// <param name="list">移除指定服务器缓存</param>
        public static void RemovAllCache(ArrayList list)
        {
            mc.FlushAll(list);
        }
        #endregion

        #region 是否存在(Exists)
        /// <summary>
        /// 判断指定键的缓存是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static bool IsExists(string key)
        {
            return mc.KeyExists(key);
        }
        #endregion

        #region 数值增减

        #region 存储一个数值元素
        /// <summary>
        /// 存储一个数值元素
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static bool StoreCounter(string key, long counter)
        {
            return mc.StoreCounter(key, counter);
        }
        /// <summary>
        ///  存储一个数值元素
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="inc">增长幅度</param>
        /// <param name="hashCode">哈希码</param>
        /// <returns></returns>
        public static bool StoreCounter(string key, long counter, int hashCode)
        {
            return mc.StoreCounter(key, counter, hashCode);
        }
        #endregion

        #region 获取一个数值元素
        /// <summary>
        /// 获取一个数值元素
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static long GetCounter(string key)
        {
            return mc.GetCounter(key);
        }
        /// <summary>
        ///  获取一个数值元素
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashCode">哈希码</param>
        /// <returns></returns>
        public static long GetCounter(string key, int hashCode)
        {
            return mc.GetCounter(key, hashCode);
        }
        #endregion

        #region 增加一个数值元素的值(Increment)
        /// <summary>
        /// 将一个数值元素增加。 如果元素的值不是数值类型，将其作为0处理
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static long Increment(string key)
        {
            return mc.Increment(key);
        }
        /// <summary>
        ///  将一个数值元素增加。 如果元素的值不是数值类型，将其作为0处理
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="inc">增长幅度</param>
        /// <returns></returns>
        public static long Increment(string key, long inc)
        {
            return mc.Increment(key, inc);
        }
        /// <summary>
        ///  将一个数值元素增加。 如果元素的值不是数值类型，将其作为0处理
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="inc">增长幅度</param>
        /// <param name="hashCode">哈希码</param>
        /// <returns></returns>
        public static long Increment(string key, long inc, int hashCode)
        {
            return mc.Increment(key, inc, hashCode);
        }
        #endregion

        #region 减小一个数值元素的值(Decrement)
        /// <summary>
        /// 减小一个数值元素的值，减小多少由参数offset决定。 如果元素的值不是数值，以0值对待。如果减小后的值小于0,则新的值被设置为0
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static long Decrement(string key)
        {
            return mc.Decrement(key);
        }
        /// <summary>
        ///  减小一个数值元素的值，减小多少由参数offset决定。 如果元素的值不是数值，以0值对待。如果减小后的值小于0,则新的值被设置为0
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="inc">增长幅度</param>
        /// <returns></returns>
        public static long Decrement(string key, long inc)
        {
            return mc.Decrement(key, inc);
        }
        /// <summary>
        ///  减小一个数值元素的值，减小多少由参数offset决定。 如果元素的值不是数值，以0值对待。如果减小后的值小于0,则新的值被设置为0
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="inc">增长幅度</param>
        /// <param name="hashCode">哈希码</param>
        /// <returns></returns>
        public static long Decrement(string key, long inc, int hashCode)
        {
            return mc.Decrement(key, inc, hashCode);
        }
        #endregion

        #endregion

        #endregion

        #region 指定服务器

        #region 获取(Get)

        /// <summary>
        /// 从指定服务器获取
        /// </summary>
        /// <param name="server">服务器，Svr1，Svr2</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static object GetFrom(string server, string key)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.Get(key);
        }
        /// <summary>
        /// 从指定服务器获取
        /// </summary>
        /// <param name="server">服务器，Svr1，Svr2</param>
        /// <param name="key">键</param>
        /// <param name="hashCode">哈希码</param>
        public static object GetFrom(string server, string key, int hashCode)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.Get(key, hashCode);
        }
        /// <summary>
        /// 从指定服务器获取
        /// </summary>
        /// <param name="server">服务器，Svr1，Svr2</param>
        /// <param name="key">键</param>
        /// <param name="asString">是否把值作为字符串返回</param>
        public static object GetFrom(string server, string key, object value, bool asString)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.Get(key, value, asString);
        }
        #endregion

        #region 写入(Set)
        /// <summary>
        ///  设置数据缓存
        /// </summary>
        /// <param name="server">服务器，格式为Svr1，Svr2，Svr3，对应配置文件host</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static bool SetTo(string server, string key, object value)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.Set(key, value);
        }
        /// <summary>
        ///  设置数据缓存
        /// </summary>
        /// <param name="server">服务器，格式为Svr1，Svr2，Svr3，对应配置文件host</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="hashCode">哈希码</param>
        public static bool SetTo(string server, string key, object value, int hashCode)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.Set(key, value, hashCode);
        }
        /// <summary>
        ///  设置数据缓存
        /// </summary>
        /// <param name="server">服务器，格式为Svr1，Svr2，Svr3，对应配置文件host</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        public static bool SetTo(string server, string key, object value, DateTime expiry)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.Set(key, value, expiry);
        }
        /// <summary>
        ///  设置数据缓存
        /// </summary>
        /// <param name="server">服务器，格式为Svr1，Svr2，Svr3，对应配置文件host</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        public static bool SetTo(string server, string key, object value, DateTime expiry, int hashCode)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.Set(key, value, expiry, hashCode);
        }
        #endregion

        #region 批量写(Set)
        /// <summary>
        /// 批量设置数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetMultipleTo(string server, string[] keys, object[] values)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            for (int i = 0; i < keys.Length; i++)
            {
                client.Set(keys[i], values[i]);
            }
        }
        /// <summary>
        /// 批量设置数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="hashCode">哈希码</param>
        public static void SetMultipleTo(string server, string[] keys, object[] values, int[] hashCodes)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            for (int i = 0; i < keys.Length; i++)
            {
                client.Set(keys[i], values[i], hashCodes[i]);
            }
        }
        /// <summary>
        /// 批量设置数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        public static void SetMultipleTo(string server, string[] keys, object[] values, DateTime[] expirys)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            for (int i = 0; i < keys.Length; i++)
            {
                client.Set(keys[i], values[i], expirys[i]);
            }
        }
        /// <summary>
        /// 批量设置数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        public static void SetMultipleTo(string server, string[] keys, object[] values, DateTime[] expirys, int[] hashCodes)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            for (int i = 0; i < keys.Length; i++)
            {
                client.Set(keys[i], values[i], expirys[i], hashCodes[i]);
            }
        }
        #endregion

        #region 批量读取(Multiple),返回哈希表 Hashtable
        /// <summary>
        /// 批量读取数据缓存
        /// </summary>
        /// <param name="keys">键集合</param>
        public static Hashtable GetMultipleFrom(string server, string[] keys)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.GetMultiple(keys);
        }
        /// <summary>
        /// 批量读取数据缓存
        /// </summary>
        /// <param name="keys">键集合</param>
        /// <param name="hashCodes">哈希码集合</param>
        public static Hashtable GetMultipleFrom(string server, string[] keys, int[] hashCodes)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.GetMultiple(keys, hashCodes);
        }
        /// <summary>
        /// 批量读取数据缓存
        /// </summary>
        /// <param name="keys">键集合</param>
        /// <param name="hashCodes">哈希码集合</param>
        /// <param name="asString">所有值返回字符</param>
        public static Hashtable GetMultipleFrom(string server, string[] keys, int[] hashCodes, bool asString)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.GetMultiple(keys, hashCodes, asString);
        }
        #endregion

        #region 批量读取(Multiple),返回对象数组object[]
        /// <summary>
        /// 批量读取数据缓存
        /// </summary>
        /// <param name="keys">键集合</param>
        public static object[] GetMultipleArrayFrom(string server, string[] keys)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.GetMultipleArray(keys);
        }
        /// <summary>
        /// 批量读取数据缓存
        /// </summary>
        /// <param name="keys">键集合</param>
        /// <param name="hashCodes">哈希码集合</param>
        public static object[] GetMultipleArrayFrom(string server, string[] keys, int[] hashCodes)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.GetMultipleArray(keys, hashCodes);
        }
        /// <summary>
        /// 批量读取数据缓存
        /// </summary>
        /// <param name="keys">键集合</param>
        /// <param name="hashCodes">哈希码集合</param>
        /// <param name="asString">所有值返回字符</param>
        public static object[] GetMultipleArrayFrom(string server, string[] keys, int[] hashCodes, bool asString)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.GetMultipleArray(keys, hashCodes, asString);
        }
        #endregion

        #region 批量读取(Multiple),返回泛型集合List[T]
        /// <summary>
        /// 批量读取数据缓存
        /// </summary>
        /// <param name="keys">键集合</param>
        public static List<T> GetMultipleListFrom<T>(string server, string[] keys)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            object[] obj = client.GetMultipleArray(keys);
            List<T> list = new List<T>();
            foreach (object o in obj)
            {
                list.Add((T)o);
            }
            return list;
        }
        /// <summary>
        /// 批量读取数据缓存
        /// </summary>
        /// <param name="keys">键集合</param>
        /// <param name="hashCodes">哈希码集合</param>
        public static List<T> GetMultipleListFrom<T>(string server, string[] keys, int[] hashCodes)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            object[] obj = client.GetMultipleArray(keys, hashCodes);
            List<T> list = new List<T>();
            foreach (object o in obj)
            {
                list.Add((T)o);
            }
            return list;
        }
        /// <summary>
        /// 批量读取数据缓存
        /// </summary>
        /// <param name="keys">键集合</param>
        /// <param name="hashCodes">哈希码集合</param>
        /// <param name="asString">所有值返回字符</param>
        public static List<T> GetMultipleListFrom<T>(string server, string[] keys, int[] hashCodes, bool asString)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            object[] obj = client.GetMultipleArray(keys, hashCodes, asString);
            List<T> list = new List<T>();
            foreach (object o in obj)
            {
                list.Add((T)o);
            }
            return list;
        }
        #endregion

        #region 替换更新(Replace)
        /// <summary>
        /// 替换更新数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void ReplaceFrom(string server, string key, object value)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            client.Replace(key, value);
        }
        /// <summary>
        /// 替换更新数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="hashCode">哈希码</param>
        public static void ReplaceFrom(string server, string key, object value, int hashCode)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            client.Replace(key, value, hashCode);
        }
        /// <summary>
        /// 替换更新数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        public static void ReplaceFrom(string server, string key, object value, DateTime expiry)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            client.Replace(key, value, expiry);
        }
        /// <summary>
        /// 替换更新数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        public static void ReplaceFrom(string server, string key, object value, DateTime expiry, int hashCode)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            client.Replace(key, value, expiry, hashCode);
        }
        #endregion

        #region 删除(Delete)

        /// <summary>
        ///删除指定条件缓存
        /// </summary>
        /// <param name="key">键</param>
        public static bool DeleteFrom(string server, string key)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.Delete(key);
        }
        /// <summary>
        /// 删除指定条件缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashCode">哈希码</param>
        /// <param name="expiry">过期时间</param>
        public static bool DeleteFrom(string server, string key, int hashCode, DateTime expiry)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.Delete(key, hashCode, expiry);
        }
        /// <summary>
        /// 删除指定条件缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="expiry">过期时间</param>
        public static bool DeleteFrom(string server, string key, DateTime expiry)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.Delete(key, expiry);
        }

        /// <summary>
        /// 移除全部缓存
        /// </summary>
        public static bool RemovAllCacheFrom(string server)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.FlushAll();
        }
        /// <summary>
        /// 移除全部缓存
        /// </summary>
        /// <param name="list">移除指定服务器缓存</param>
        public static bool RemovAllCacheFrom(string server, ArrayList list)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.FlushAll(list);
        }
        #endregion

        #region 是否存在(Exists)
        /// <summary>
        /// 判断指定键的缓存是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static bool IsExists(string server, string key)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.KeyExists(key);
        }
        #endregion

        #region 数值增减

        #region 存储一个数值元素
        /// <summary>
        /// 存储一个数值元素
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static bool StoreCounterTo(string server, string key, long counter)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.StoreCounter(key, counter);
        }
        /// <summary>
        ///  存储一个数值元素
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="inc">增长幅度</param>
        /// <param name="hashCode">哈希码</param>
        /// <returns></returns>
        public static bool StoreCounterTo(string server, string key, long counter, int hashCode)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.StoreCounter(key, counter, hashCode);
        }
        #endregion

        #region 获取一个数值元素
        /// <summary>
        /// 获取一个数值元素
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static long GetCounterFrom(string server, string key)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.GetCounter(key);
        }
        /// <summary>
        ///  获取一个数值元素
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashCode">哈希码</param>
        /// <returns></returns>
        public static long GetCounterFrom(string server, string key, int hashCode)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.GetCounter(key, hashCode);
        }
        #endregion

        #region 增加一个数值元素的值(Increment)
        /// <summary>
        /// 将一个数值元素增加。 如果元素的值不是数值类型，将其作为0处理
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static long IncrementTo(string server, string key)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.Increment(key);
        }
        /// <summary>
        ///  将一个数值元素增加。 如果元素的值不是数值类型，将其作为0处理
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="inc">增长幅度</param>
        /// <returns></returns>
        public static long IncrementTo(string server, string key, long inc)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.Increment(key, inc);
        }
        /// <summary>
        ///  将一个数值元素增加。 如果元素的值不是数值类型，将其作为0处理
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="inc">增长幅度</param>
        /// <param name="hashCode">哈希码</param>
        /// <returns></returns>
        public static long IncrementTo(string server, string key, long inc, int hashCode)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.Increment(key, inc, hashCode);
        }
        #endregion

        #region 减小一个数值元素的值(Decrement)
        /// <summary>
        /// 减小一个数值元素的值，减小多少由参数offset决定。 如果元素的值不是数值，以0值对待。如果减小后的值小于0,则新的值被设置为0
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static long DecrementFrom(string server, string key)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.Decrement(key);
        }
        /// <summary>
        ///  减小一个数值元素的值，减小多少由参数offset决定。 如果元素的值不是数值，以0值对待。如果减小后的值小于0,则新的值被设置为0
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="inc">增长幅度</param>
        /// <returns></returns>
        public static long DecrementFrom(string server, string key, long inc)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.Decrement(key, inc);
        }
        /// <summary>
        ///  减小一个数值元素的值，减小多少由参数offset决定。 如果元素的值不是数值，以0值对待。如果减小后的值小于0,则新的值被设置为0
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="inc">增长幅度</param>
        /// <param name="hashCode">哈希码</param>
        /// <returns></returns>
        public static long DecrementFrom(string server, string key, long inc, int hashCode)
        {
            MemcachedClient client = GetClient(server);
            client.PoolName = server;
            return client.Decrement(key, inc, hashCode);
        }
        #endregion

        #endregion

        #endregion
    }
    /// <summary>
    /// Stats命令行参数
    /// </summary>
    public enum Stats
    {
        /// <summary>
        /// stats : 显示服务器信息, 统计数据等
        /// </summary>
        Default = 0,
        /// <summary>
        /// stats reset : 清空统计数据
        /// </summary>
        Reset = 1,
        /// <summary>
        /// stats malloc : 显示内存分配数据
        /// </summary>
        Malloc = 2,
        /// <summary>
        /// stats maps : 显示"/proc/self/maps"数据
        /// </summary>
        Maps = 3,
        /// <summary>
        /// stats sizes
        /// </summary>
        Sizes = 4,
        /// <summary>
        /// stats slabs : 显示各个slab的信息,包括chunk的大小,数目,使用情况等
        /// </summary>
        Slabs = 5,
        /// <summary>
        /// stats items : 显示各个slab中item的数目和最老item的年龄(最后一次访问距离现在的秒数)
        /// </summary>
        Items = 6,
        /// <summary>
        /// stats cachedump slab_id limit_num : 显示某个slab中的前 limit_num 个 key 列表
        /// </summary>
        CachedDump = 7,
        /// <summary>
        /// stats detail [on|off|dump] : 设置或者显示详细操作记录   on:打开详细操作记录  off:关闭详细操作记录 dump: 显示详细操作记录(每一个键值get,set,hit,del的次数)
        /// </summary>
        Detail = 8,
        /// <summary>
        /// 基本状态信息
        /// </summary>
        States = 9
    }
}
