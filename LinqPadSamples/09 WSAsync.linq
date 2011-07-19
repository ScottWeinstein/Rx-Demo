<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft Reactive Extensions SDK\v1.0.10425\Binaries\.NETFramework\v4.0\System.Reactive.dll</Reference>
  <Namespace>System.Reactive.Concurrency</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>System.Reactive</Namespace>
  <Namespace>System.Reactive.Subjects</Namespace>
  <Namespace>System.Reactive.Joins</Namespace>
  <Namespace>System.Net</Namespace>
</Query>

public IObservable<string> GetWebData(Uri uri)
{
	// or use FromAsyncPattern on a webservice ref
	var wc = new System.Net.WebClient();
	var xs = Observable.FromEventPattern<DownloadStringCompletedEventHandler,DownloadStringCompletedEventArgs>(eh => wc.DownloadStringCompleted += eh, eh => wc.DownloadStringCompleted -= eh)
			   .Select(epArgs => epArgs.EventArgs.Result);
	wc.DownloadStringAsync(uri);
	return xs;

}
void Main()
{
	var xs = GetWebData(new Uri("http://google.com"));
	
	xs.Select(str => str.Substring(0,10)).Take(1).Dump();
	xs.Select(str => str.Substring(10,10)).Take(1).Dump();

	
}

