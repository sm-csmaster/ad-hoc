using BestStories.Retriever.Application.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace BestStories.Retriever.Application.Tests
{
    [TestFixture]
    public class CacheServiceTests
    {
        private Mock<IMemoryCache> _memoryCacheMock;
        private CacheService _cacheService;

        [SetUp]
        public void SetUp()
        {
            _memoryCacheMock = new Mock<IMemoryCache>();
            _cacheService = new CacheService(_memoryCacheMock.Object);
        }

        [Test]
        public void Get_ReturnsValue_WhenKeyExists()
        {
            // Arrange
            var key = "testKey";
            var expectedValue = "testValue";
            object expectedValueObj = expectedValue;
            _memoryCacheMock
                .Setup(m => m.TryGetValue(key, out expectedValueObj))
                .Returns(true);

            // Act
            var result = _cacheService.Get<string>(key);

            // Assert
            Assert.That(result, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Get_ReturnsDefault_WhenKeyDoesNotExist()
        {
            // Arrange
            var key = "testKey";
            object expectedValue = null;
            _memoryCacheMock.Setup(m => m.TryGetValue(key, out expectedValue)).Returns(false);

            // Act
            var result = _cacheService.Get<string>(key);

            // Assert
            Assert.IsNull(result);
        }
    }
}
