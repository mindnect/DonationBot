using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace JSAssist
{
	internal class Profiler
	{
		private Dictionary<string, Profiler.StopwatchMax> mapWatch = new Dictionary<string, Profiler.StopwatchMax>();

		public Profiler()
		{
		}

		public void Print()
		{
			Console.WriteLine("\n\nProfiler");
			Console.WriteLine("=====================================");
			foreach (KeyValuePair<string, Profiler.StopwatchMax> keyValuePair in this.mapWatch)
			{
				Console.WriteLine(string.Concat(new object[] { "[", keyValuePair.Key, "]\t\t", keyValuePair.Value.max }));
			}
		}

		public void Start(string name)
		{
			if (!this.mapWatch.ContainsKey(name))
			{
				Profiler.StopwatchMax stopwatchMax = new Profiler.StopwatchMax();
				this.mapWatch.Add(name, stopwatchMax);
			}
			this.mapWatch[name].Start();
		}

		public void Stop(string name)
		{
			this.mapWatch[name].Stop();
		}

		private class StopwatchMax
		{
			private Stopwatch watch;

			public long max;

			public StopwatchMax()
			{
				this.watch = new Stopwatch();
				this.max = (long)0;
			}

			public void Start()
			{
				this.watch.Reset();
				this.watch.Start();
			}

			public void Stop()
			{
				this.watch.Stop();
				if (this.max < this.watch.ElapsedMilliseconds)
				{
					this.max = this.watch.ElapsedMilliseconds;
				}
			}
		}
	}
}