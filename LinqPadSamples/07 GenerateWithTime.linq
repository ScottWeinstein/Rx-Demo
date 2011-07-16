<Query Kind="Expression">
  <Reference Relative="..\Packages\Rx-Main.1.0.10621\lib\Net4\System.Reactive.dll">&lt;Personal&gt;\d\GitHub\Rx-Demo\Packages\Rx-Main.1.0.10621\lib\Net4\System.Reactive.dll</Reference>
  <Namespace>System.Reactive.Linq</Namespace>
</Query>

Observable.Generate(

	 0,  // initalState

	ii => ii < 10, // condition

	ii => ii + 1,  // interate

	ii => "item " + ii, // result selector
	
	ii => TimeSpan.FromMilliseconds(ii * 200) // timeselector
	
	)