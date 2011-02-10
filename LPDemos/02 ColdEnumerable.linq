<Query Kind="Statements">
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2838.0\Net4\System.CoreEx.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2838.0\Net4\System.Interactive.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2838.0\Net4\System.Reactive.dll</Reference>
</Query>

var rnd = new Random();
var qry = Enumerable.Range(1,5)
		  .Select(ii=> new {Index=ii, Random = rnd.Next()});

qry.Dump();
qry.Where(item => item.Index ==1).Dump();
		  