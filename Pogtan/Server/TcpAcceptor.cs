using System.Net;
using System.Net.Sockets;
using Pogtan.Packet;

namespace Pogtan.Server;

public class TcpAcceptor(Server server, int port) : IDisposable
{
    private readonly TcpListener listener = new(IPAddress.Any, port);

    public void Dispose() => listener.Dispose();

    public async Task Start()
    {
        listener.Server.NoDelay = true;
        listener.Start();
        Console.WriteLine($"TcpAcceptor listening on port : {port}");
        try
        {
            while (true)
            {
                TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                _ = HandleClient(tcpClient);
            }
        }
        finally
        {
            listener.Stop();
        }
    }

    public async Task HandleClient(TcpClient tcpClient)
    {
        using Client client = new(server, tcpClient, Random.Shared.Next(HeaderConverter.HeaderTypeMax));

        Console.WriteLine("Client connected");
        server.OnConnect(client);
        using (SendPacket p = new(SendPacketType.Connect))
        {
            p.Encode2(ServerConfig.GameVersion); // MinimumGameVersion
            p.Encode2(ServerConfig.GameVersion); // ServerGameVersion
            p.Encode4(client.HeaderType); // PacketHeaderType
            p.EncodeStr(""); // UrlPatch
            client.Write(p);
        }

        try
        {
            while (true)
            {
                await client.Read();
            }
        }
        catch (EndOfStreamException)
        {
            Console.WriteLine("Client disconnected");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception while handling client : {ex}");
        }
        finally
        {
            server.OnDisconnect(client);
        }
    }
}
