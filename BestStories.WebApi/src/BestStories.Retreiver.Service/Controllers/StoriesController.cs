using BestStories.Retriever.Application.Abstractions;
using BestStories.Retriever.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BestStories.Retriever.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoriesController : ControllerBase
    {
        private readonly ILogger<StoriesController> _logger;
        private readonly IStoryService _storyService;

        public StoriesController(ILogger<StoriesController> logger, IStoryService storyService)
        {
            _logger = logger;
            _storyService = storyService;
        }

        [HttpGet("beststories/{numberOfStories}")]
        public async Task<IEnumerable<Story>> Get(int numberOfStories)
        {
            _logger.LogInformation("Starting to download stories content.");

            return await _storyService.GetBestStoriesAsync(numberOfStories);
        }
    }
}
