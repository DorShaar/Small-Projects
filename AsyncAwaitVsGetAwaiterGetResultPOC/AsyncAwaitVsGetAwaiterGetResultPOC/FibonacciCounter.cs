using System.Threading;
using System.Threading.Tasks;

namespace Threads
{
    internal class FibonacciCounter
    {
        public long Count(CancellationToken cancellationToken)
        {
            long num1 = 0;
            long num2 = 1;
            long sum = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                Task.Delay(500).GetAwaiter().GetResult();
                sum = num1 + num2;
                
                num2 = num1;
                num1 = sum;
            }

            return sum;
        }

        public async Task<long> CountAsync(CancellationToken cancellationToken)
        {
            long num1 = 0;
            long num2 = 1;
            long sum = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(500).ConfigureAwait(false);
                sum = num1 + num2;
                num2 = num1;
                num1 = sum;
            }

            return sum;
        }
    }
}