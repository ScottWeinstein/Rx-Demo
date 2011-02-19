<Query Kind="Expression">
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2856.0\Net4\System.CoreEx.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2856.0\Net4\System.Interactive.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2856.0\Net4\System.Reactive.dll</Reference>
</Query>

Observable.CreateWithDisposable<string>( obs => 
	{
		var disp = new System.Disposables.BooleanDisposable();
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