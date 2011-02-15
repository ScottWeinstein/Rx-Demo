<Query Kind="Expression">
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2856.0\Net4\System.CoreEx.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2856.0\Net4\System.Interactive.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2856.0\Net4\System.Reactive.dll</Reference>
</Query>

Observable.GenerateWithTime(

	 0,  // initalState

	ii => ii < 10, // condition

	ii => ii + 1,  // interate

	ii => "item " + ii, // result selector
	
	ii => TimeSpan.FromMilliseconds(ii * 200) // timeselector
	
	)
