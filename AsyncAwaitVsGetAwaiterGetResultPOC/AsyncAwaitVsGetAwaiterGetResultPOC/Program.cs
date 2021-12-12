using BenchmarkDotNet.Running;
using System;
using System.Threading.Tasks;

namespace Threads
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<AsyncAwaitPOC>();

            //AsyncAwaitPOC asyncAwaitPOC = new AsyncAwaitPOC();

            //Console.WriteLine("Hello World!");
            //asyncAwaitPOC.Start();

            //await Task.Delay(1000);

            //Console.WriteLine("Start Async!");
            //await asyncAwaitPOC.StartAsync().ConfigureAwait(false);
        }
    }
}
