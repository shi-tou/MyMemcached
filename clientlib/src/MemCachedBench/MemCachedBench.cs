/**
 * MemcachedBench.cs
 *
 * Copyright (c) 2005
 * Tim Gebhardt <tim@gebhardtcomputing.com>
 * 
 * Based off of code written by
 * Greg Whalin <greg@meetup.com>
 * for his Java Memcached client:
 * http://www.whalin.com/memcached/
 * 
 *
 * See the memcached website:
 * http://www.danga.com/memcached/
 *
 * This module is Copyright (c) 2005 Tim Gebhardt.
 * All rights reserved.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 *
 * This library is distributed in the hope that it will be
 * useful, but WITHOUT ANY WARRANTY; without even the implied
 * warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
 * PURPOSE.  See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307  USA
 *
 * @author Tim Gebhardt<tim@gebhardtcomputing.com> 
 * @version 1.0
 */
namespace Memcached.MemcachedBench
{
	using System;
	using System.Collections;

	using Memcached.ClientLibrary;

	public class MemcachedBench 
	{
		/// <summary>
		/// Arguments: 
		///		arg[0] = the number of runs to do
		///		arg[1] = the run at which to start benchmarking
		/// </summary>
		/// <param name="args"></param>
		[STAThread]
		public static void Main(String[] args) 
		{
			int runs = 100;
			int start = 200;
			if(args.Length > 1)
			{
				runs = int.Parse(args[0]);
				start = int.Parse(args[1]);
			}

			string[] serverlist = { "140.192.34.72:11211", "140.192.34.73:11211" };

			// initialize the pool for memcache servers
			SockIOPool pool = SockIOPool.GetInstance();
			pool.SetServers(serverlist);

			pool.InitConnections = 3;
			pool.MinConnections = 3;
			pool.MaxConnections = 5;

			pool.SocketConnectTimeout = 1000;
            pool.SocketTimeout = 3000;

			pool.MaintenanceSleep = 30;
			pool.Failover = true;

			pool.Nagle = false;
			pool.Initialize();

			// initialize the pool for memcache servers
//			SockIOPool pool = SockIOPool.Instance;
//			pool.Servers = serverlist;
//
//			pool.InitConn = 5;
//			pool.MinConn = 5;
//			pool.MaxConn = 50;
//			pool.MaintSleep = 30;
//			pool.SocketTO = 1000;
//
//			pool.Nagle = false;
//			pool.Initialize();

//
//			// get client instance
			MemcachedClient mc = new MemcachedClient();
			mc.EnableCompression = false;

//			MemcachedClient mc = new MemcachedClient();
//			mc.CompressEnable = false;
//			mc.CompressThreshold = 0;
//			mc.Serialize = true;

			string keyBase = "testKey";
			string obj = "This is a test of an object blah blah es, serialization does not seem to slow things down so much.  The gzip compression is horrible horrible performance, so we only use it for very large objects.  I have not done any heavy benchmarking recently";

			long begin = DateTime.Now.Ticks;
			for(int i = start; i < start+runs; i++) 
			{
				mc.Set(keyBase + i, obj);
			}
			long end = DateTime.Now.Ticks;
			long time = end - begin;

			Console.WriteLine(runs + " sets: " + new TimeSpan(time).ToString() + "ms");

			begin = DateTime.Now.Ticks;
			int hits = 0;
			int misses = 0;
			for(int i = start; i < start+runs; i++) 
			{
				string str = (string) mc.Get(keyBase + i);
				if(str != null)
					++hits;
				else
					++misses;
			}
			end = DateTime.Now.Ticks;
			time = end - begin;

			Console.WriteLine(runs + " gets: " + new TimeSpan(time).ToString() + "ms");
			Console.WriteLine("Cache hits: " + hits.ToString());
			Console.WriteLine("Cache misses: " + misses.ToString());

			IDictionary stats = mc.Stats();
			foreach(string key1 in stats.Keys)
			{
				Console.WriteLine(key1);
				Hashtable values = (Hashtable)stats[key1];
				foreach(string key2 in values.Keys)
				{
					Console.WriteLine(key2 + ":" + values[key2]);
				}
				Console.WriteLine();
			}

			SockIOPool.GetInstance().Shutdown();
		}
	}
}