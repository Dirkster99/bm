namespace BreadcrumbLib.Utils
{
	using System;
	using System.Threading.Tasks;

	public static class AsyncUtils
	{
		/// <summary>
		/// Runs a task  returned from an async method
		/// as background thread. This can be useful if
		/// the caller does not want or cannot implement
		/// an additional await to continue processing.
		/// 
		/// The utility function can be used to avoid compiler warnings
		/// when executing an async method directly without additional await.
		/// </summary>
		/// <param name="task"></param>
		public static void RunAsync(Func<Task> task)
		{
			Task tsk = task();

			if (tsk.Status == TaskStatus.Created)
				Task.Run(() => tsk);
		}
	}
}
