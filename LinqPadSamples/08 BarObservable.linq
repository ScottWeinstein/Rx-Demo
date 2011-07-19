<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft Reactive Extensions SDK\v1.0.10425\Binaries\.NETFramework\v4.0\System.Reactive.dll</Reference>
  <Namespace>System.Reactive.Concurrency</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>System.Reactive</Namespace>
  <Namespace>System.Reactive.Subjects</Namespace>
  <Namespace>System.Reactive.Joins</Namespace>
</Query>


public class Person
{
	public string Name { get; set; }
	public int Age { get; set; }
	public string Sex { get; set; }
}

void Main()
{
	Subject<Person> bar = new Subject<Person>();
	IObservable<Person> peopleinBar = bar;
	IObserver<Person> barEntrance = bar;
	
	Func<IObservable<Person>,string,IObservable<int>> getCounts = 
		(xs,sex) => xs.Where(p => p.Sex == sex).Scan(0, (count, p) => count + 1);
		
	var countofMen = getCounts(peopleinBar,"M");
	var countOfWomen = peopleinBar.Where(p => p.Sex == "F").Scan(0, (count, p) => count + 1);
	var MFRatio = countofMen.CombineLatest(countOfWomen, (m,w) => (double)m/w);
	
	MFRatio.Dump();
	
	barEntrance.OnNext(new Person { Age = 20, Name="Joe", Sex = "M" });
	barEntrance.OnNext(new Person { Age = 21, Name="Jane", Sex = "F" });
	Thread.Sleep(500);
	barEntrance.OnNext(new Person { Age = 21, Name="Andy", Sex = "M" });
	Thread.Sleep(500);
	barEntrance.OnNext(new Person { Age = 22, Name="Pinky", Sex = "F" });
	Thread.Sleep(500);
	barEntrance.OnNext(new Person { Age = 24, Name="Athena", Sex = "F" });
	
}


// Define other methods and classes here
