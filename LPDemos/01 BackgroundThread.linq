<Query Kind="Statements">
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2838.0\Net4\System.CoreEx.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2838.0\Net4\System.Interactive.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2838.0\Net4\System.Reactive.dll</Reference>
</Query>

string.Format("Main thread {0}",Thread.CurrentThread.ManagedThreadId).Dump();

Observable
		.Interval(TimeSpan.FromSeconds(0.3))
		.Take(20)
		.Select(_ => string.Format("RX thread   {0}",Thread.CurrentThread.ManagedThreadId))
		.Dump();
		
		