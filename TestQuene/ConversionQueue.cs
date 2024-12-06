using System.Collections.Concurrent;
using System.Threading;

namespace TestQuene
{
    public class ConversionQueue
    {
        // ## 非同步限流器
        private readonly SemaphoreSlim _semaphore;

        // ## 隊伍
        // BlockingCollection ref => https://blog.darkthread.net/blog/blockingcollection/
        // 單執行緒Queue，多執行緒ConcurrentQueue，多執行緒+限制元素數量BlockingCollection => https://learn.microsoft.com/zh-tw/dotnet/standard/collections/thread-safe/when-to-use-a-thread-safe-collection
        private readonly BlockingCollection<Func<Task>> _taskQueue = new();

        public ConversionQueue(int maxCount)
        {
            // https://stackoverflow.com/questions/4706734/semaphore-what-is-the-use-of-initial-count
            // new SemaphoreSlim(0, 2); //all threadpool threads wait
            // new SemaphoreSlim(1, 2); //only one thread has access to the resource at a time
            // new SemaphoreSlim(2, 2); //two threadpool threads can access the resource concurrently
            _semaphore = new SemaphoreSlim(maxCount, maxCount);

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
