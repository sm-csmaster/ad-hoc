using BestStories.Retriever.Application.Abstractions;
using BestStories.Retriever.Application.DTOs;
using BestStories.Retriever.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BestStories.Retriever.Application.Services
{
    public class StoryService : IStoryService
    {
        private readonly HttpClient _httpClient;
        private readonly ExternalApiSettings _settings;
        private readonly ICacheService _cacheService;
        private readonly ILogger<StoryService> _logger;

        private const string CacheKey = "BestStories";

        public StoryService(
            IHttpClientFactory httpClientFactory, 
            IOptions<ExternalApiSettings> settings, 
            ICacheService cacheService, 
            ILogger<StoryService> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _settings = settings.Value;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<IEnumerable<Story>> GetBestStoriesAsync(int numberOfStories)
        {
            var storyIds = await GetBestStoriesIdsAsync();

            var tasks = storyIds.Take(numberOfStories).Select(async id =>
            {
                try
                {
                    return await GetStoryByIdAsync(id);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error fetching story with ID {id}: {ex.Message}");
                    return null; // Return null or handle as needed
                }
            });

            var stories = await Task.WhenAll(tasks);

            // Filter out any null stories
            // Order by score in descending order
            return stories.Where(s => s != null).OrderByDescending(s => s.Score).ToArray();
        }

        private async Task<List<int>> GetBestStoriesIdsAsync()
        {
            var storyIds = _cacheService.Get<List<int>>(CacheKey);
            if (storyIds is not null)
            {
                return storyIds;
            }

            var bestStoriesResponse = await _httpClient.GetStringAsync(_settings.BestStoriesUrl);
            storyIds = JsonSerializer.Deserialize<List<int>>(bestStoriesResponse);
            _cacheService.Set(CacheKey, storyIds, TimeSpan.FromMinutes(30));

            return storyIds;
        }

        private async Task<Story> GetStoryByIdAsync(int id)
        {
            var storyCacheKey = $"Story_{id}";
            var story = _cacheService.Get<Story>(storyCacheKey);
            if (story is not null)
            {
                return story;
            }

            var storyResponse = await _httpClient.GetStringAsync($"{_settings.StoryDetailsUrl}/{id}.json");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            story = JsonSerializer.Deserialize<Story>(storyResponse, options);
            _cacheService.Set(storyCacheKey, story, TimeSpan.FromMinutes(30));

            return story;
        }
    }
}
