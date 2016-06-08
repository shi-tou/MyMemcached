/**
/// UnitTests.java
/// Test class for testing memcached C# client.
///
/// Copyright (c) 2005
/// Tim Gebhardt
/// Based off of code written by Kevin Burton
/// for the Memcached Java client found here: 
/// http://www.whalin.com/memcached/
///
/// See the memcached website:
/// http://www.danga.com/memcached/
///
/// This module is Copyright (c) 2005 Tim Gebhardt
/// All rights reserved.
///
/// This library is free software; you can redistribute it and/or
/// modify it under the terms of the GNU Lesser General Public
/// License as published by the Free Software Foundation; either
/// version 2.1 of the License, or (at your option) any later
/// version.
///
/// This library is distributed in the hope that it will be
/// useful, but WITHOUT ANY WARRANTY; without even the implied
/// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
/// PURPOSE.  See the GNU Lesser General Public License for more
/// details.
///
/// You should have received a copy of the GNU Lesser General Public
/// License along with this library; if not, write to the Free Software
/// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307  USA
**/

namespace Memcached.UnitTests
{
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.Text;
	using Memcached.ClientLibrary;

	public class UnitTests 
	{
		public static MemcachedClient mc  = null;
		public static MemcachedClient mc2 = null;

		public static void test1() 
		{
			mc.Set("foo", true);
			bool b = (bool)mc.Get("foo");
			Debug.Assert(b);
		}

		public static void test2() 
		{
			mc.Set("foo", int.MaxValue);
			int i = (int)mc.Get("foo");
			Debug.Assert(i == int.MaxValue);
		}

		public static void test3() 
		{
			string input = "test of string encoding";
			mc.Set("foo", input);
			string s = (string)mc.Get("foo");
			Debug.Assert(s == input);
		}
    
		public static void test4() 
		{
			mc.Set("foo", 'z');
			char c = (char)mc.Get("foo");
			Debug.Assert(c == 'z');
		}

		public static void test5() 
		{
			mc.Set("foo", (byte)127);
			byte b = (byte)mc.Get("foo");
			Debug.Assert(b == 127);
		}

		public static void test6() 
		{
			mc.Set("foo", new StringBuilder("hello"));
			StringBuilder o = (StringBuilder)mc.Get("foo");
			Debug.Assert(o.ToString() == "hello");
		}

		public static void test7() 
		{
			mc.Set("foo", (short)100);
			short o = (short)mc.Get("foo");
			Debug.Assert(o == 100);
		}

		public static void test8() 
		{
			mc.Set("foo", long.MaxValue);
			long o = (long)mc.Get("foo");
			Debug.Assert(o == long.MaxValue);
		}

		public static void test9() 
		{
			
			mc.Set("foo", 1.1d);
			double o = (double)mc.Get("foo");
			Debug.Assert(o == 1.1d);
		}

		public static void test10() 
		{
			
			mc.Set("foo", 1.1f);
			float o = (float)mc.Get("foo");
			Debug.Assert(o == 1.1f);
		}

		public static void test11() 
		{
			mc.Delete("foo");
			mc.Set("foo", 100, DateTime.Now);
			System.Threading.Thread.Sleep(1000);
			Debug.Assert(mc.Get("foo") != null);
		}

		public static void test12() 
		{
			long i = 0;
			mc.StoreCounter("foo", i);
			mc.Increment("foo"); // foo now == 1
			mc.Increment("foo", (long)5); // foo now == 6
			long j = mc.Decrement("foo", (long)2); // foo now == 4
			Debug.Assert(j == 4);
			Debug.Assert(j == mc.GetCounter("foo"));
		}

		public static void test13() 
		{
			DateTime d1 = new DateTime();
			mc.Set("foo", d1);
			DateTime d2 = (DateTime) mc.Get("foo");
			Debug.Assert(d1 == d2);
		}

		public static void test14() 
		{
			if(mc.KeyExists("foobar123"))
				mc.Delete("foobar123");
			Debug.Assert(!mc.KeyExists("foobar123"));
			mc.Set("foobar123", 100000);
			Debug.Assert(mc.KeyExists("foobar123"));

			if(mc.KeyExists("counterTest123"))
				mc.Delete("counterTest123");
			Debug.Assert(!mc.KeyExists("counterTest123"));
			mc.StoreCounter("counterTest123", 0);
			Debug.Assert(mc.KeyExists("counterTest123"));
		}

		/// <summary>
		/// This runs through some simple tests of the MemCacheClient.
		/// 
		/// Command line args:
		/// args[0] = number of threads to spawn
		/// args[1] = number of runs per thread
		/// args[2] = size of object to store 
		/// </summary>
		/// <param name="args">the command line arguments</param>
		[STAThread]
		public static void Main(string[] args) 
		{
			String[] serverlist = { "140.192.34.72:11211", "140.192.34.73:11211"  };

			// initialize the pool for memcache servers
			SockIOPool pool = SockIOPool.GetInstance("test");
			pool.SetServers(serverlist);
			pool.Initialize();

			mc = new MemcachedClient();
			mc.PoolName = "test";
			mc.EnableCompression = false;

			test1();
			test2();
			test3();
			test4();
			test5();
			test6();
			test7();
			test8();
			test9();
			test10();
			test11();
			test12();
			test13();
			test14();

			pool.Shutdown();
		}
	}
}