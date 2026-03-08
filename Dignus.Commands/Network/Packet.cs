using Dignus.Sockets.Interfaces;
using System.Text;

namespace Dignus.Commands.Network
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
