namespace BreadcrumbTestLib.Utils
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// By - Stephen Toub - MSFT
    /// http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/10266988.aspx
    /// </summary>
    public class AsyncLock
    {
        #region fields
        private readonly AsyncSemaphore m_semaphore;
        private readonly Task<Releaser> m_releaser;
        #endregion fields

        #region constructors
        public AsyncLock()
        {
            this.m_semaphore = new AsyncSemaphore(1);
            this.m_releaser = Task.FromResult(new Releaser(this));
        }
        #endregion constructors

        #region methods
        public Task<Releaser> LockAsync()
        {
            var wait = this.m_semaphore.WaitAsync();
            return wait.IsCompleted ?
                this.m_releaser :
                wait.ContinueWith((_, state) => new Releaser((AsyncLock)state),
                    this, CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }
        #endregion methods

        public struct Releaser : IDisposable
        {
            private readonly AsyncLock m_toRelease;

            /// <summary>
            /// Struct Constructor
            /// </summary>
            /// <param name="toRelease"></param>
            internal Releaser(AsyncLock toRelease)
            {
                this.m_toRelease = toRelease;
            }

            public void Dispose()
            {
                if (this.m_toRelease != null)
                    this.m_toRelease.m_semaphore.Release();
            }
        }
    }
}
