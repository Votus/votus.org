using System;
using System.IO;
using System.Text;

namespace Votus.Core.Infrastructure.Serialization
{
    public interface ISerializer
    {
        string Serialize(object obj);
        byte[] SerializeToBytes(object obj, Encoding encoding);

        T Deserialize<T>(Stream stream);
        T Deserialize<T>(string serializedObject);
        T Deserialize<T>(byte[] serializedObject, Encoding encoding);
        
        object Deserialize(string serializedObject, Type type);
        object Deserialize(string serializedObject, string type);
    }
}