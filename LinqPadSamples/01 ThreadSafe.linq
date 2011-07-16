<Query Kind="Statements">
  <Reference Relative="..\Packages\Rx-Main.1.0.10621\lib\Net4\System.Reactive.dll">&lt;Personal&gt;\d\GitHub\Rx-Demo\Packages\Rx-Main.1.0.10621\lib\Net4\System.Reactive.dll</Reference>
  <Namespace>System.Reactive.Linq</Namespace>
</Query>

Observable
		.Interval(TimeSpan.FromSeconds(0.1))
		.Take(5)
		.Do(_ => Thread.Sleep(1500))
		.Dump();