<Query Kind="Statements">
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2838.0\Net4\System.CoreEx.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2838.0\Net4\System.Interactive.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Microsoft Cloud Programmability\Reactive Extensions\v1.0.2838.0\Net4\System.Reactive.dll</Reference>
</Query>

Func<IObservable<long>,TimeSpan,IObservable<bool>> IsConnected = ( heartbeats,timeout) =>
{
	var connected = Observable.Return(true);
	var disconnected = Observable.Return(false).Delay(timeout); 

	return Observable.Switch
			(
				heartbeats.Select(hb=> connected.Concat(disconnected))
			)
			.DistinctUntilChanged();
};

var server =Observable.Interval(TimeSpan.FromSeconds(0.5)).Take(2).Concat(Observable.Interval(TimeSpan.FromSeconds(3)).Take(2));
IsConnected(server, TimeSpan.FromSeconds(2)).Dump();