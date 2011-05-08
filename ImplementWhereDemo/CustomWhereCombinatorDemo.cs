namespace RXDemo
{
    using System;
    using System.Linq;
    using System.Diagnostics;
    using System.Reactive.Linq;

    public class CustomWhereCombinatorDemo
    {
        public static void Main(string[] args)
        {
            Observable.Range(1, 200)
                        //.Where(val => val > 190)
                        .CustomWherePedantic(val => val > 190)
                        //.CustomWhereLessPedantic(val => val > 190)
                        //.CustomWhere(val => val > 190)
                      .Subscribe(Console.WriteLine);

            Console.WriteLine("press any key");
            Console.ReadKey();
        }
    }
}