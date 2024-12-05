using System.Collections.Concurrent;

namespace TestQuene
{
    public class ConversionQueue
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly BlockingCollection<Func<Task>> _taskQueue = new BlockingCollection<Func<Task>>();

        public ConversionQueue()
        {
            Task.Run(async () =>
            {
                foreach (var task in _taskQueue.GetConsumingEnumerable())
                {
                    await _semaphore.WaitAsync();
                    try
                    {
                        await task();
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }
            });
        }

        public Task<T> Enqueue<T>(Func<Task<T>> taskGenerator)
        {
            return Task.Run(async () =>
            {
                await _semaphore.WaitAsync();
                try
                {
                    return await taskGenerator();
                }
                finally
                {
                    _semaphore.Release();
                }
            });
        }
    }
}
