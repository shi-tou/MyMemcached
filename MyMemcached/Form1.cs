using MyMemcached.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyMemcached
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Memcached服务配置
        /// </summary>
        public ServersConfigInfo config;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //加载配置
            config = ServersConfig.GetConfig();
            foreach (Server s in config.Servers)
            {
                this.comboBox_Servers.Items.Add(s.PoolName);
            }
            this.comboBox_Servers.SelectedIndex = 0;
        }
        /// <summary>
        /// 删除所有缓存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Server s in config.Servers)
            {
                MemcacheHelper.RemovAllCacheFrom(s.PoolName);
            }
        }
        /// <summary>
        /// 写入读取指定服务池
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if(this.txtKey.Text.Trim()=="" || this.txtValue.Text.Trim()=="")
            {
                MessageBox.Show("请输入Key/Value。");
                return;
            }
            MemcacheHelper.SetTo(this.comboBox_Servers.SelectedItem.ToString(), this.txtKey.Text, this.txtValue.Text);
        }
        /// <summary>
        /// 读取指定服务池
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (this.txtKey.Text.Trim() == "" )
            {
                MessageBox.Show("请输入Key。");
                return;
            }
            this.txtValue.Text = Convert.ToString(MemcacheHelper.GetFrom(this.comboBox_Servers.SelectedItem.ToString(), this.txtKey.Text));
        }

        /// <summary>
        /// 分布式写入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (this.txtKey.Text.Trim() == "" || this.txtValue.Text.Trim() == "")
            {
                MessageBox.Show("请输入Key/Value。");
                return;
            }
            MemcacheHelper.SetTo(GetServer(this.txtKey.Text), this.txtKey.Text, this.txtValue.Text);
        }

        /// <summary>
        /// 分布式读取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (this.txtKey.Text.Trim() == "")
            {
                MessageBox.Show("请输入Key。");
                return;
            }
            this.txtValue.Text = Convert.ToString(MemcacheHelper.GetFrom(GetServer(this.txtKey.Text), this.txtKey.Text));
        }

        /// <summary>
        /// 利用条数Hash算法,取分布式服务
        /// MemCache虽然被称为"分布式缓存"，但是MemCache本身完全不具备分布式的功能，
        /// MemCache集群之间不会相互通信（与之形成对比的，比如JBoss Cache，某台服务
        /// 器有缓存数据更新时，会通知集群中其他机器更新缓存或清除缓存数据），所谓的
        /// "分布式"，完全依赖于客户端程序的实现，就像上面这张图的流程一样。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetServer(string key)
        {
            int hsCode = key.GetHashCode();
            int index = Math.Abs(hsCode % config.Servers.Count);
            return config.Servers[index].PoolName;
        }

        
    }
}
