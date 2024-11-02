using BestStories.Retriever.Application.Abstractions;
using BestStories.Retriever.Application.Services;
using BestStories.Retriever.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace BestStories.Retriever.Application.Tests
{
    public class StoryServiceTests
    {
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private Mock<ICacheService> _cacheServiceMock;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private IOptions<ExternalApiSettings> _settings;
        private Mock<ILogger<StoryService>> _loggerMock;
        private StoryService _storyService;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<StoryService>>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _cacheServiceMock = new Mock<ICacheService>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _settings = Options.Create(new ExternalApiSettings
            {
                BestStoriesUrl = "https://api.example.com/beststories",
                StoryDetailsUrl = "https://api.example.com/story"
            });

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _storyService = new StoryService(_httpClientFactoryMock.Object, _settings, _cacheServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetBestStoriesAsync_HandlesEmptyResponse()
        {
            // Arrange
            var storyIds = new List<int>();
            _cacheServiceMock
                .Setup(x => x.Get<List<int>>(It.IsAny<string>()))
                .Returns((List<int>?)null);
            _httpMessageHandlerMock
                .SetupRequest(HttpMethod.Get, _settings.Value.BestStoriesUrl)
                .ReturnsResponse(JsonSerializer.Serialize(storyIds), "application/json");

            // Act
            var result = await _storyService.GetBestStoriesAsync(3);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetBestStoriesIdsAsync_HandlesInvalidJsonResponse()
        {
            // Arrange
            _cacheServiceMock
                .Setup(x => x.Get<List<int>>(It.IsAny<string>()))
                .Returns((List<int>?)null);
            _httpMessageHandlerMock
                .SetupRequest(HttpMethod.Get, _settings.Value.BestStoriesUrl)
                .ReturnsResponse("invalid json", "application/json");

            // Act & Assert
            Assert.ThrowsAsync<JsonException>(async () => await _storyService.GetBestStoriesAsync(3));
        }

    }

    public static class HttpMessageHandlerExtensions
    {
        public static Mock<HttpMessageHandler> SetupRequest(this Mock<HttpMessageHandler> handler, HttpMethod method, string url)
        {
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == method && req.RequestUri.ToString() == url),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(string.Empty)
                });

            return handler;
        }

        public static Mock<HttpMessageHandler> ReturnsResponse(this Mock<HttpMessageHandler> handler, string content, string mediaType)
        {
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(content, System.Text.Encoding.UTF8, mediaType)
                });

            return handler;
        }
    }
}
