namespace Integration.Service;

public class MemoryLockService
{
    private static readonly Dictionary<string, bool> Locks = new Dictionary<string, bool>();
    private static readonly object LockObject = new object();

    public static bool AcquireLock(string key)
    {
        lock (LockObject)
        {
            if (Locks.ContainsKey(key))
            {
                return false;
            }

            Locks[key] = true;
            return true;
        }
    }

    public static void ReleaseLock(string key)
    {
        lock (LockObject)
        {
            if (Locks.ContainsKey(key))
            {
                Locks.Remove(key);
            }
        }
    }
}
