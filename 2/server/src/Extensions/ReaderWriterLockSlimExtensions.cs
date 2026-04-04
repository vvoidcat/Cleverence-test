namespace Locks.Extensions;

internal static class ReaderWriterLockSlimExtensions
{
	public static T ResolveReadLock<T>(this ReaderWriterLockSlim rw, Func<T> func)
	{
		rw.EnterReadLock();

		try
		{
			return func();
		}
		finally
		{
			rw.ExitReadLock();
		}
	}

	public static void ResolveWriteLock(this ReaderWriterLockSlim rw, Action action)
	{
		rw.EnterWriteLock();

		try
		{
			action();
		}
		finally
		{
			rw.ExitWriteLock();
		}
	}
}
