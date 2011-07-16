<Query Kind="Expression">
  <Reference Relative="..\Packages\Rx-Main.1.0.10621\lib\Net4\System.Reactive.dll">&lt;Personal&gt;\d\GitHub\Rx-Demo\Packages\Rx-Main.1.0.10621\lib\Net4\System.Reactive.dll</Reference>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>System.Reactive.Disposables</Namespace>
</Query>

Observable.Create<string>(obs => 
	{
		var disp = new BooleanDisposable();
		if (!disp.IsDisposed)
			obs.OnNext("hello");
		if (!disp.IsDisposed)
			obs.OnNext("hello");
		if (!disp.IsDisposed)
			obs.OnNext("hello");
		
		if (!disp.IsDisposed)
			obs.OnCompleted();
		
		return disp;
	})