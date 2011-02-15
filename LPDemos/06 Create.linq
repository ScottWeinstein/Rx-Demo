<Query Kind="Expression">
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2856.0\Net4\System.CoreEx.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2856.0\Net4\System.Interactive.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2856.0\Net4\System.Reactive.dll</Reference>
</Query>

Observable.Create<string>( obs => 
	{
		obs.OnNext("hello");
		obs.OnNext("hello");
		obs.OnNext("hello");
		obs.OnCompleted();
		
		return () => {};
	})
