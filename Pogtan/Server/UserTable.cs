namespace Pogtan.Server;

public class UserTable
{
    private readonly Dictionary<string, User> dictById = new();
    private readonly Dictionary<uint, User> dictBySn = new();
    private readonly ReaderWriterLockSlim rwLock = new();

    public void Insert(User user)
    {
        rwLock.EnterWriteLock();
        try
        {
            dictBySn[user.Sn] = user;
            dictById[user.Id] = user;
        }
        finally
        {
            rwLock.ExitWriteLock();
        }
    }

    public User? GetBySn(uint sn)
    {
        rwLock.EnterReadLock();
        try
        {
            return dictBySn.TryGetValue(sn, out User? user) ? user : null;
        }
        finally
        {
            rwLock.ExitReadLock();
        }
    }

    public User? GetById(string id)
    {
        rwLock.EnterReadLock();
        try
        {
            return dictById.TryGetValue(id, out User? user) ? user : null;
        }
        finally
        {
            rwLock.ExitReadLock();
        }
    }
}
