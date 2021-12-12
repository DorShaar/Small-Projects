using BenchmarkDotNet.Attributes;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Threads
{
    [ThreadingDiagnoser]
    public class AsyncAwaitPOC
    {
        [Benchmark]
        public void Start()
        {
            int countTasksNumber = 35;

            FibonacciCounter fibonacciCounter = new();
            FileService fileService = new();
            Task<long>[] countTasks = new Task<long>[countTasksNumber];
            Task[] fileReadTasks = new Task[3];
            Task[] fileWriteTasks = new Task[3];
            string[] filePaths = new string[3];


            try
            {
                CancellationTokenSource cts = new();

                for (int i = 0; i < countTasksNumber; i++)
                {
                    System.Console.WriteLine($"Start counting {i}");
                    countTasks[i] = Task.Run(() => fibonacciCounter.Count(cts.Token));
                }

                string fileName = Path.GetRandomFileName();
                System.Console.WriteLine($"Start writing to {fileName}");
                filePaths[0] = fileName;
                fileWriteTasks[0] = Task.Run(() => fileService.WriteFile(filePaths[0], 250 * 1024 * 1024));

                fileName = Path.GetRandomFileName();
                System.Console.WriteLine($"Start writing to {fileName}");
                filePaths[1] = fileName;
                fileWriteTasks[1] = Task.Run(() => fileService.WriteFile(filePaths[1], 250 * 1024 * 1024));

                fileName = Path.GetRandomFileName();
                System.Console.WriteLine($"Start writing to {fileName}");
                filePaths[2] = fileName;
                fileWriteTasks[2] = Task.Run(() => fileService.WriteFile(filePaths[2], 250 * 1024 * 1024));

                System.Console.WriteLine("Waiting for files to finish writing");

                Task.WaitAll(fileWriteTasks);

                fileName = filePaths[0];
                System.Console.WriteLine($"Start reading from {fileName}");
                fileReadTasks[0] = Task.Run(() => fileService.ReadFile(filePaths[0]));

                fileName = filePaths[1];
                System.Console.WriteLine($"Start reading from {fileName}");
                fileReadTasks[1] = Task.Run(() => fileService.ReadFile(filePaths[1]));

                fileName = filePaths[2];
                System.Console.WriteLine($"Start reading from {fileName}");
                fileReadTasks[2] = Task.Run(() => fileService.ReadFile(filePaths[2]));

                System.Console.WriteLine("Waiting for files to finish reading");

                Task.WaitAll(fileReadTasks);

                cts.Cancel();

                long maxCount = 0;
                for (int i = 0; i < countTasksNumber; i++)
                {
                    if (maxCount < countTasks[i].Result)
                    {
                        maxCount = countTasks[i].Result;
                    }
                }

                System.Console.WriteLine($"Maximal count: {maxCount}");
            }
            finally
            {
                File.Delete(filePaths[0]);
                File.Delete(filePaths[1]);
                File.Delete(filePaths[2]);
            }
        }

        [Benchmark]
        public Task StartAsync()
        {
            int countTasksNumber = 35;

            FibonacciCounter fibonacciCounter = new();
            FileService fileService = new();
            Task<long>[] countTasks = new Task<long>[countTasksNumber];
            Task[] fileReadTasks = new Task[3];
            Task[] fileWriteTasks = new Task[3];
            string[] filePaths = new string[3];

            try
            {
                CancellationTokenSource cts = new();

                for (int i = 0; i < countTasksNumber; i++)
                {
                    System.Console.WriteLine($"Start counting async {i}");
                    countTasks[i] = fibonacciCounter.CountAsync(cts.Token);
                }

                for (int i = 0; i < 3; i++)
                {
                    filePaths[i] = Path.GetRandomFileName();
                    fileWriteTasks[i] = fileService.WriteFileAsync(filePaths[i], 250 * 1024 * 1024);
                }

                System.Console.WriteLine("Waiting for files to finish writing");

                Task.WaitAll(fileWriteTasks);

                for (int i = 0; i < 3; i++)
                {
                    fileReadTasks[i] = fileService.ReadFileAsync(filePaths[i]);
                }

                System.Console.WriteLine("Waiting for files to finish reading");

                Task.WaitAll(fileReadTasks);

                cts.Cancel();

                long maxCount = 0;
                for (int i = 0; i < countTasksNumber; i++)
                {
                    if (maxCount < countTasks[i].Result)
                    {
                        maxCount = countTasks[i].Result;
                    }
                }

                System.Console.WriteLine($"Maximal count: {maxCount}");
                return Task.CompletedTask;
            }
            finally
            {
                File.Delete(filePaths[0]);
                File.Delete(filePaths[1]);
                File.Delete(filePaths[2]);
            }
        }
    }
}