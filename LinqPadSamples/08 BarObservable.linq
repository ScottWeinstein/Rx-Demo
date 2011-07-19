<Query Kind="Program">
  <Reference Relative="..\Packages\Rx-Main.1.0.10621\lib\Net4\System.Reactive.dll">&lt;Personal&gt;\d\github\Rx-Demo\Packages\Rx-Main.1.0.10621\lib\Net4\System.Reactive.dll</Reference>
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
	
		
	var countofMen = peopleinBar
					.Where(p => p.Sex == "M")
					.Scan(0, (count, p) => count + 1);
					
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

//	Func<IObservable<Person>,string,IObservable<int>> getCounts = 
//		(xs,sex) => xs.Where(p => p.Sex == sex).Scan(0, (count, p) => count + 1);

// Define other methods and classes here