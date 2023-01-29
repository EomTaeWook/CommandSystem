using CLISystem.Net.Protocol.Handler;
using Kosher.Log;
using Kosher.Sockets.Interface;
using System.Text;

namespace CLISystem.Net.Serializer
{
    internal class PacketDeserializer : IPacketDeserializer
    {
        ICollection<IProtocolHandler> _handlers;
        public PacketDeserializer(ICollection<IProtocolHandler> handlers)
        {
            _handlers = handlers;
        }
        public void Deserialize(BinaryReader stream)
        {
            var bodyBytes = stream.ReadBytes((int)stream.BaseStream.Length);


            var text = Encoding.UTF8.GetString(bodyBytes);
            LogHelper.Debug($"Receive Deserialize : {text}");

            foreach(var item in _handlers)
            {
                
            }
        }

        public bool IsTakedCompletePacket(BinaryReader stream)
        {
            if (stream.BaseStream.CanRead == false)
            {
                return false;
            }


            LogHelper.Debug($"[{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff")}] buffer size : {stream.BaseStream.Length}");

            return true;
        }
    }
}
