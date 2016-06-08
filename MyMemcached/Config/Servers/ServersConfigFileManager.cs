using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MyMemcached.Config
{
    /// <summary>
    /// 文件配置管理基类
    /// </summary>
    public class ServersConfigFileManager : DefaultConfigFileManager
    {
        private static ServersConfigInfo m_configinfo;

        /// <summary>
        /// 文件修改时间
        /// </summary>
        private static DateTime m_fileoldchange;

        /// <summary>
        /// 初始化文件修改时间和对象实例
        /// </summary>
        static ServersConfigFileManager()
        {
            m_fileoldchange = System.IO.File.GetLastWriteTime(ConfigFilePath);
            m_configinfo = (ServersConfigInfo)DefaultConfigFileManager.DeserializeInfo(ConfigFilePath, typeof(ServersConfigInfo));
        }

        /// <summary>
        /// 当前的配置实例
        /// </summary>
        public new static IConfigInfo ConfigInfo
        {
            get { return m_configinfo; }
            set { m_configinfo = (ServersConfigInfo)value; }
        }


        /// <summary>
        /// 配置文件所在路径
        /// </summary>
        public static string filename = null;


        /// <summary>
        /// 获取配置文件所在路径
        /// </summary>
        public new static string ConfigFilePath
        {
            get
            {
                if (filename == null)
                {
                    filename = Application.StartupPath + "\\Config\\Servers.config";
                }

                return filename;
            }
        }

        /// <summary>
        /// 返回配置类实例
        /// </summary>
        /// <returns></returns>
        public static ServersConfigInfo LoadConfig()
        {
            ConfigInfo = LoadConfig(ref m_fileoldchange, ConfigFilePath, ConfigInfo);
            return ConfigInfo as ServersConfigInfo;
        }

        /// <summary>
        /// 保存配置类实例
        /// </summary>
        /// <returns></returns>
        public override bool SaveConfig()
        {
            return base.SaveConfig(ConfigFilePath, ConfigInfo);
        }
    }
}
