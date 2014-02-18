using System.IO;
using Votus.Core.Infrastructure.Serialization;
using Xunit;

namespace Votus.Testing.Unit.Core.Infrastructure.Serialization
{
    public abstract class ISerializerTests
    {
        private readonly ISerializer _serializer;

        protected
        ISerializerTests(
            ISerializer serializer)
        {
            _serializer = serializer;
        }

        [Fact]
        public 
        void 
        Serialize_InstanceIsSerializable_ReturnsSerializedObject()
        {
            // Arrange
            var testObject = new SerializableObject {
                IntProperty    = 123,
                StringProperty = "456"
            };

            // Act
            var actualValue = _serializer.Serialize(testObject);

            // Assert
            Assert.Equal("{\"IntProperty\":123,\"StringProperty\":\"456\"}", actualValue);
        }

        [Fact]
        public
        void
        Deserialize_StringIsDeserializable_ReturnsObject()
        {
            // Arrange
            const string serializedObject = "{\"IntProperty\":123,\"StringProperty\":\"456\"}";

            // Act
            var actual = _serializer.Deserialize<SerializableObject>(serializedObject);

            // Assert
            Assert.Equal(123, actual.IntProperty);
        }

        [Fact]
        public 
        void 
        Deserialize_StreamIsDeserializable_ReturnsObject()
        {
            // Arrange
            const string serializedObject = "{\"IntProperty\":123,\"StringProperty\":\"456\"}";

            var memoryStream = new MemoryStream();
            
            new StreamWriter(memoryStream) { AutoFlush = true }
                .Write(serializedObject);

            // Act
            var actual = _serializer.Deserialize<SerializableObject>(memoryStream);

            // Assert
            Assert.Equal(123, actual.IntProperty);            
        }

        [Fact]
        public 
        void 
        Deserialize_TypeIsSpecified_ReturnsObject()
        {
            // Arrange
            const string serializedObject = "{\"IntProperty\":123,\"StringProperty\":\"456\"}";

            // Act
            var actual = (SerializableObject)_serializer.Deserialize(serializedObject, typeof(SerializableObject));

            // Assert
            Assert.Equal(123, actual.IntProperty);
        }

        [Fact]
        public
        void 
        Deserialize_TypeStringIsValid_ReturnsObject()
        {
            // Arrange
            var objectTypeName            = typeof(SerializableObject).AssemblyQualifiedName;
            const string serializedObject = "{\"IntProperty\":123,\"StringProperty\":\"456\"}";

            // Act
            var actual = (SerializableObject)_serializer.Deserialize(
                serializedObject, 
                objectTypeName
            );

            // Assert
            Assert.Equal(123, actual.IntProperty);
        }

        class SerializableObject
        {
            public int      IntProperty     { get; set; }
            public string   StringProperty  { get; set; }
        }
    }
}
