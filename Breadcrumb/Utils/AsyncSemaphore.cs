namespace Breadcrumb.Utils
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	/// <summary>
	/// By - Stephen Toub - MSFT
	/// http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/10266983.aspx
	/// </summary>
	public class AsyncSemaphore
	{
		private static readonly Task S_Completed = Task.FromResult(true);
		private readonly Queue<TaskCompletionSource<bool>> m_waiters = new Queue<TaskCompletionSource<bool>>();
		private int m_currentCount;

		public AsyncSemaphore(int initialCount)
		{
			if (initialCount < 0)
				throw new ArgumentOutOfRangeException("initialCount");

			this.m_currentCount = initialCount;
		}

		public Task WaitAsync()
		{
			lock (this.m_waiters)
			{
				if (this.m_currentCount > 0)
				{
					--this.m_currentCount;
					return S_Completed;
				}
				else
				{
					var waiter = new TaskCompletionSource<bool>();

					this.m_waiters.Enqueue(waiter);
					
					return waiter.Task;
				}
			}
		}

		public void Release()
		{
			TaskCompletionSource<bool> toRelease = null;

			lock (this.m_waiters)
			{
				if (this.m_waiters.Count > 0)
					toRelease = this.m_waiters.Dequeue();
				else
					++this.m_currentCount;
			}

			if (toRelease != null)
				toRelease.SetResult(true);
		}
	}
}
