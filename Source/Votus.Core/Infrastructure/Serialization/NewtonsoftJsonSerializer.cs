using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Votus.Core.Infrastructure.Serialization
{
    public class NewtonsoftJsonSerializer : ISerializer
    {
        private readonly JsonSerializer _serializer = new JsonSerializer();

        public 
        NewtonsoftJsonSerializer(
            Formatting jsonFormatting = Formatting.Indented)
        {
            _serializer.Formatting = jsonFormatting;
        }

        public
        string
        Serialize(
            object obj)
        {
            var stringBuilder = new StringBuilder();

            _serializer.Serialize(
                new JsonTextWriter(new StringWriter(stringBuilder)),
                obj
            );

            return stringBuilder.ToString();
        }

        public 
        byte[]
        SerializeToBytes(
            object      obj, 
            Encoding    encoding)
        {
            var serializedObject = Serialize(obj);
            return encoding.GetBytes(serializedObject);
        }

        public 
        T 
        Deserialize<T>(
            Stream stream)
        {
            string streamContent;

            stream.Position = 0;

            using (var reader = new StreamReader(stream))
                streamContent = reader.ReadToEnd();

            return Deserialize<T>(streamContent);
        }

        public 
        T
        Deserialize<T>(
            string serializedObject)
        {
            return (T)Deserialize(serializedObject, typeof(T));
        }

        public
        T
        Deserialize<T>(
            byte[]      serializedObject, 
            Encoding    encoding)
        {
            return Deserialize<T>(encoding.GetString(serializedObject));
        }

        public 
        object 
        Deserialize(
            string  serializedObject, 
            Type    type)
        {
            return _serializer.Deserialize(
                new JsonTextReader(
                    new StringReader(serializedObject)
                ),
                type
            );
        }

        public
            object
            Deserialize(
            string serializedObject,
            string type)
        {
            return Deserialize(
                serializedObject, 
                Type.GetType(type)
            );
        }
    }
}
