namespace Pogtan.Server;

public class ChannelInfo
{
    public ChannelInfo(int id, string name, ChannelType type)
    {
        Id = id;
        Name = name;
        Type = type;
    }

    public int Id { get; }
    public string Name { get; }
    public ChannelType Type { get; }
}
