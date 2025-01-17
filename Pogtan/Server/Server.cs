using System.Net;
using System.Threading.Channels;
using Pogtan.Packet;

namespace Pogtan.Server;

public abstract class Server : IDisposable
{
    protected readonly Channel<Action> TaskQueue;
    protected readonly TcpAcceptor TcpAcceptor;
    protected readonly UdpAcceptor UdpAcceptor;
    protected readonly UserTable UserTable;

    protected Server(int port)
    {
        TaskQueue = Channel.CreateUnbounded<Action>();
        TcpAcceptor = new TcpAcceptor(this, port);
        UdpAcceptor = new UdpAcceptor(this, port + 1);
        UserTable = new UserTable();
    }

    public void Dispose()
    {
        TaskQueue.Writer.Complete();
        TcpAcceptor.Dispose();
        UdpAcceptor.Dispose();
    }

    public abstract void OnConnect(Client client);

    public abstract void OnDisconnect(Client client);

    public abstract void HandlePacket(ReceivedPacketType packetType, ReceivedPacket packet, Client client);

    public abstract void HandlePacket(UdpPacketType packetType, ReceivedPacket packet, IPEndPoint endpoint);

    public async Task Start()
    {
        _ = TcpAcceptor.Start();
        _ = UdpAcceptor.Start();
        await foreach (Action task in TaskQueue.Reader.ReadAllAsync())
        {
            try
            {
                task();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while processing task : {ex}");
            }
        }
    }

    public async Task AddTask(Action task) => await TaskQueue.Writer.WriteAsync(task);
}
