<Query Kind="Expression">
  <Reference Relative="..\Packages\Rx-Main.1.0.10621\lib\Net4\System.Reactive.dll">&lt;Personal&gt;\d\GitHub\Rx-Demo\Packages\Rx-Main.1.0.10621\lib\Net4\System.Reactive.dll</Reference>
  <Namespace>System.Reactive.Linq</Namespace>
</Query>

Observable.Create<string>( obs => 
	{
		obs.OnNext("hello");
		obs.OnNext("hello");
		obs.OnNext("hello");
		obs.OnCompleted();
		
		return () => {};
	})