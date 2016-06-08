using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyMemcached.Config
{
    /// <summary>
    /// Memcachede服务配置信息类
    /// </summary>
    public class ServersConfigInfo : IConfigInfo
    {
        /// <summary>
        /// 服务群
        /// </summary>
        [XmlArray(ElementName = "Servers"), XmlArrayItem(ElementName = "Server")]
        public List<Server> Servers;
        /// <summary>
        /// 默认连接池
        /// </summary>
        public string DefaultServer;
        /// <summary>
        /// 初始连接数 
        /// </summary>
        public int InitConnections;
        /// <summary>
        /// 最小连接数 
        /// </summary>
        public int MinConnections;
        /// <summary>
        /// 最大连接数 
        /// </summary>
        public int MaxConnections;
        /// <summary>
        /// 设置连接的套接字超时(毫秒)
        /// </summary>
        public int SocketConnectTimeout;
        /// <summary>
        /// 设置套接字超时读取(毫秒)
        /// </summary>
        public int SocketTimeout;
        /// <summary>
        /// //设置维护线程运行的睡眠时间。
        /// 如果设置为0，那么维护线程将不会启动,如果设置30就是每隔30秒醒来一次  
        /// </summary>
        public int MaintenanceSleep;
        /// <summary>
        /// 获取或设置池的故障标志。  
        /// 如果这个标志被设置为true则socket连接失败，将试图从另一台服务器返回一个套接字如果存在的话。  
        /// 如果设置为false，则得到一个套接字如果存在的话。否则返回NULL，如果它无法连接到请求的服务器。  
        /// </summary>
        public bool Failover;
        /// <summary>
        /// 如果为false，对所有创建的套接字关闭Nagle的算法  
        /// </summary>
        public bool Nagle;

    }
    public class Server
    {
        /// <summary>
        /// 连接池名称
        /// </summary>
        [XmlAttribute(AttributeName = "poolName")]
        public string PoolName;
        /// <summary>
        /// IP
        /// </summary>
        [XmlAttribute(AttributeName = "ip")]
        public string IP;
        /// <summary>
        /// 端口
        /// </summary>
        [XmlAttribute(AttributeName = "port")]
        public int Port;
    }
}
