<Query Kind="Statements">
  <Reference Relative="..\Packages\Rx-Main.1.0.10621\lib\Net4\System.Reactive.dll">&lt;Personal&gt;\d\GitHub\Rx-Demo\Packages\Rx-Main.1.0.10621\lib\Net4\System.Reactive.dll</Reference>
  <Namespace>System.Reactive.Linq</Namespace>
</Query>

string.Format("Main thread {0}",Thread.CurrentThread.ManagedThreadId).Dump();

Observable
		.Interval(TimeSpan.FromSeconds(0.1)) // every 100 MS
		.Take(20)							 // end after 20
		.Select(_ => { return string.Format("RX thread    {0}", Thread.CurrentThread.ManagedThreadId); }) // print the thread this is running on
		.Dump(); // and print to LinqPad