using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMemcached
{
    /// <summary>
    /// 单例模式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Singleton<T> where T : new()
    {
        private Singleton() { }
        public static T Instance
        {
            get { return SingletonCreator.instance; }
        }

        internal class SingletonCreator
        {
            static SingletonCreator() { }
            internal static readonly T instance = new T();
        }
    }
}
