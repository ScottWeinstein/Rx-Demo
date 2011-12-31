author: @ScottWeinstein
title: Reactive Extension 1.0
footer: @ScottWeinstein
subfooter: Lab49
presdate: 7/16/2011

#LinqPad Demos

1. [BackgroundThread](LinqPadSamples/01 BackgroundThread.linq)
1. [ThreadSafe](LinqPadSamples/01 ThreadSafe.linq)
1. [ColdEnumerable]("LinqPadSamples/02 ColdEnumerable.linq")
1. [ColdObservable]("LinqPadSamples/03 ColdObservable.linq")
1. [HotObservable]("LinqPadSamples/04 HotObservable.linq")
1. [Create]("LinqPadSamples/06 Create.linq")
1. [CreateDisposable]("LinqPadSamples/06 CreateDisposable.linq")
1. [GenerateWithTime]("LinqPadSamples/07 GenerateWithTime.linq")

# VisualStudio Demos
### Querying a server

1. A slow or expensive query
1. On a background loop
1. With user initiated action, for the impatient
1. But not too often
1. Compare the ad-hoc w/ the Rx approach

###UI for password check

1. Accept correct password
1. Reject incorrect
1. No “submit button”
1. Timeout
1. Must be unit testable

### Twitter client
1. Need to make an IO from a traditional API
1. Want a mock version for UI development
1. Only want to show latest for each user
1. Want late subscribers to have latest values

### Streaming Olap


Agenda
===================================================================
* What's the Rx thing about, why should I care?
* Will it make my life easier?
* What's new in 1.0

What's the Rx thing about, why should I care?
=======================================================================
* For some, Rx is something of a revelation.
* A **good** way to deal with asynchronous code
* A **good** way to deal with event based code
* A **good** way to deal with concurrent code
* A **good** way to deal with time sensitive code

Reactive matters
=======================================================================
* Systems and architectures that are not _reactive_ are **legacy** systems
    * Real life is async
    * Sync, ie, blocking activities are souces of real stress
        * waiting in line for coffee, the elevator, the check to clear, etc
* Batch, pull, polling, request/response were common b/c building asyncronous code was (much) harder

* Till now...

A **good** way?
=======================================================================
* Composable
* Testable
* Pure (side-effect free)

###Two use cases
1. Managing a bar
1. Calling a web service async

So what is Rx?
=======================================================================
* A new Interface pair `IObservable<T>` and `IObserver<T>` for .Net BCL
    * The **dual** of `IEnumerable<T>` and `IEnumerator<T>`
* A core implementation and a set of Linq combinators for doing useful  things

A look at some recent Rx questions
=======================================================================
* [Detect "double tap" on a smartphone](http://stackoverflow.com/questions/6714786/interpret-double-tap-from-touchlocation-stream-using-rx)
* [Update on maximum value reached (price, diskspace, water volume, temp, etc)](http://stackoverflow.com/questions/6644073/how-to-select-increasing-subsequence-of-values-from-iobservablet)
* [Detecting if a server went down](http://stackoverflow.com/questions/6162908/detect-isalive-on-an-iobservable)

Open questions?
=======================================================================


More Details - IEnumerable and IEnumerator
=======================================================================
<% code :lang => 'csharp', :line_numbers => 'off' do %> 
interface IEnumerable<T>
{	
    IEnumerator<T> GetEnumerator();
}

interface IEnumerator<T>: IDisposable 
{	
    T Current { get; } 	
    bool MoveNext();
}
<% end %> 


More Details - IObservable and IObserver
=======================================================================
<% code :lang => 'csharp', :line_numbers => 'off' do %> 
interface IObservable<T>
{	
    IDisposable Subscribe(IObserver<T> observer);
}

interface IObserver<T>
{	
    void OnCompleted();	
    void OnError(Exception error);	
    void OnNext(T value);
}
<% end %> 

Pop Quiz - Hot or Not?
=======================================================================

![JessRabbit](http://www.mycomicnetwork.com/v2/gallery/wp-content/uploads/2011/02/disney-jessica-rabbit.jpg)


Pop Quiz - Hot or Not?
=======================================================================

1. `new[] { 1, 2, 3, 4 }`
1. `new List<string>()`
1. `Enumerable.Range(1, Int32.MaxValue)`
1. `new int[Int32.MaxValue]`
1. `People.Where(pers => pers.Age >= 21)`
1. `Observable.Range(1,3)`
1. `Observable.FromEvent("Click")`


Traps
=======================================================================

* Avoid implementing IObservable from scratch
* Hot or not?


How to build
=======================================================================
##Demos

    > msbuild # compile
    > msbuild .\rx.msbuild # for unit tests

To run the twitter example, you'll need to create the authkeys.txt file and add your own twitter API authkey

## Slide deck
    slideshow -f http://github.com/geraldb/slideshow-s6-syntax-highlighter/raw/master/s6syntax.txt
    slideshow -t s6syntax.txt index.md 
