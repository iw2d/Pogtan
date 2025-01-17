namespace Pogtan.Server;

public class User
{
    public User(uint sn, string id)
    {
        Sn = sn;
        Id = id;
    }

    public uint Sn { get; }
    public string Id { get; }

    public int Level => 10;
}
