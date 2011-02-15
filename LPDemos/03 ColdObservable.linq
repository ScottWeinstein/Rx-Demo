<Query Kind="Statements">
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2856.0\Net4\System.CoreEx.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2856.0\Net4\System.Interactive.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2856.0\Net4\System.Reactive.dll</Reference>
</Query>

var rnd = new Random();
var qry = Observable
		.Range(1,3)
		.Select(ii=> new {Index=ii, Random = rnd.Next()});

qry.Where(item => item.Index == 1).Dump();
qry.Dump();
