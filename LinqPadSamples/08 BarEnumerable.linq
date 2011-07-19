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
	var peopleinBar = new [] 
	{
		new Person { Age = 20, Name="Joe", Sex = "M" },
		new Person { Age = 21, Name="Jane", Sex = "F" },
		new Person { Age = 21, Name="Andy", Sex = "M" },
		new Person { Age = 22, Name="Pinky", Sex = "F" },
		new Person { Age = 24, Name="Athena", Sex = "F" },
	};
	
	var men = peopleinBar.Where(p => p.Sex == "M");
	var women = peopleinBar.Where(p => p.Sex == "F");
	var MFRatio = ((double)men.Count())/women.Count();
	
	MFRatio.Dump();
		
}

// Define other methods and classes here
