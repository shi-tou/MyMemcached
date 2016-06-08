using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMemcached.Config
{

    /// <summary>
    /// 配置管理类接口
    /// </summary>
    public class ServersConfig
    {
        /// <summary>
        /// 获取配置类实例
        /// </summary>
        /// <returns></returns>
        public static ServersConfigInfo GetConfig()
        {
            return ServersConfigFileManager.LoadConfig();
        }

        /// <summary>
        /// 保存配置类实例
        /// </summary>
        /// <param name="emailconfiginfo"></param>
        /// <returns></returns>
        public static bool SaveConfig(ServersConfigInfo info)
        {
            ServersConfigFileManager ecfm = new ServersConfigFileManager();
            ServersConfigFileManager.ConfigInfo = info;
            return ecfm.SaveConfig();
        }
    }
}
