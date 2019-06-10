using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GameData
{
    public static class Serializer
    {
        public static byte[] Serialize(object obj)
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            formatter.Serialize(stream, obj);
            var bytes = stream.ToArray();
            stream.Close();
            stream.Dispose();
            return bytes;
        }

        public static object Deserialize(byte[] serializedAsBytes)
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            stream.Write(serializedAsBytes, 0, serializedAsBytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            var obj = formatter.Deserialize(stream);
            stream.Close();
            stream.Dispose();
            return obj;
        }
    }
}