using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using GameData.Packets;

namespace GameData
{
    public static class Network
    {
        public const int ServerPort = 1997;

        public static void SendPacket(IPacket packet, NetworkStream stream)
        {
            var serialized = Serializer.Serialize(packet);
            if (serialized.Length == 0)
                throw new Exception();
            stream.Write(serialized, 0, serialized.Length);
        }

        public static IPacket ReceivePacket(NetworkStream stream)
        {
            var data = new List<byte>();
            var buffer = new byte[1024];
            do
            {
                var bytesCount = stream.Read(buffer, 0, buffer.Length);
                data.AddRange(buffer.Take(bytesCount));
            }
            while (stream.DataAvailable);

            if (data.Count == 0)
                return null;
                //throw new Exception();
            var packet = Serializer.Deserialize(data.ToArray());
            return (IPacket) packet;
        }
    }
}