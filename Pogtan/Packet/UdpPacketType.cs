namespace Pogtan.Packet;

public enum UdpPacketType : byte
{
    // Receive
    CheckUdpComm = 0x00,
    CheckUdpPacket = 0x01,

    // Send
    CheckUdpCommResult = 0x02,
    CheckUdpPacketResult = 0x03
}
