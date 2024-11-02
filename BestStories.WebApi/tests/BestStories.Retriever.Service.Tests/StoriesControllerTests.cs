using BestStories.Retriever.Application.Abstractions;
using BestStories.Retriever.Application.DTOs;
using BestStories.Retriever.Service.Controllers;
using Microsoft.Extensions.Logging;
using Moq;

namespace BestStories.Retriever.Service.Tests
{
    [TestFixture]
    public class StoriesControllerTests
    {
        private Mock<IStoryService> _storyServiceMock;
        private Mock<ILogger<StoriesController>> _loggerMock;
        private StoriesController _storiesController;

        [SetUp]
        public void SetUp()
        {
            _storyServiceMock = new Mock<IStoryService>();
            _loggerMock = new Mock<ILogger<StoriesController>>();
            _storiesController = new StoriesController(_loggerMock.Object, _storyServiceMock.Object);
        }

        [Test]
        public async Task Get_ReturnsOkResult_WithListOfStories()
        {
            // Arrange
            var stories = new List<Story>
            {
                new Story { Id = 1, Score = 100 },
                new Story { Id = 2, Score = 200 }
            };
            _storyServiceMock
                .Setup(service => service.GetBestStoriesAsync(It.IsAny<int>()))
                .ReturnsAsync(stories);

            // Act
            var result = await _storiesController.Get(2);

            // Assert
            Assert.IsInstanceOf<IEnumerable<Story>>(result);
            Assert.AreEqual(2, ((List<Story>)result).Count);
        }

        [Test]
        public async Task Get_LogsInformationMessage()
        {
            // Arrange
            var stories = new List<Story>
            {
                new Story { Id = 1, Score = 100 },
                new Story { Id = 2, Score = 200 }
            };
            _storyServiceMock
                .Setup(service => service.GetBestStoriesAsync(It.IsAny<int>()))
                .ReturnsAsync(stories);

            // Act
            await _storiesController.Get(2);

            // Assert
            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Starting to download stories content.")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
    }
}