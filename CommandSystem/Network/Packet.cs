using Dignus.Sockets.Interfaces;
using System.Text;

namespace CommandSystem.Network
{
    public class Packet(string body) : IPacket
    {
        public byte[] Body = Encoding.UTF8.GetBytes(body);

        public int GetLength()
        {
            return Body.Length;
        }
    }
}
