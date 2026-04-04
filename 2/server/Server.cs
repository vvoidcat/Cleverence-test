using Extensions;

namespace Locks;

public static class Server
{
    private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

    private static int _count = 0;

    public static int GetCount() => _lock.ResolveReadLock(() => { return _count; });
    public static void AddToCount(int value) => _lock.ResolveWriteLock(() => { _count += value; });

    internal static void Reset() => _lock.ResolveWriteLock(() => { _count = 0; });
}