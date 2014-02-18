using Newtonsoft.Json;
using Votus.Core.Infrastructure.Serialization;

namespace Votus.Testing.Unit.Core.Infrastructure.Serialization
{
    public class NewtonsoftJsonSerializerTests : ISerializerTests
    {
        public NewtonsoftJsonSerializerTests()
            : base(new NewtonsoftJsonSerializer(Formatting.None))
        {
            
        }
    }
}
