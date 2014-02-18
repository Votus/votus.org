using Votus.Core.Infrastructure.Data;
using Xunit;

namespace Votus.Testing.Unit.Core.Infrastructure.Data
{
    public abstract class IReadableRepositoryTests
    {
        private const string ValidName  = "some-name";
        private const string ValidValue = "some-value";

        public IReadableRepository ReadableRepository { get; set; }

        protected IReadableRepositoryTests(IReadableRepository readableRepository)
        {
            ReadableRepository = readableRepository;
        }

        [Fact]
        public void Get_ValueExists_ReturnsValue()
        {
            // Arrange
            SetValue(ValidName, ValidValue);

            // Act
            var actual = ReadableRepository.Get(ValidName);

            // Assert
            Assert.Equal(actual, ValidValue);
        }

        protected abstract void SetValue(string key, string value);
    }
}
