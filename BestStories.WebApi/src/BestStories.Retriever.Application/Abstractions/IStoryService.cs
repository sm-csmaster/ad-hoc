using BestStories.Retriever.Application.DTOs;

namespace BestStories.Retriever.Application.Abstractions
{
    public interface IStoryService
    {
        Task<IEnumerable<Story>> GetBestStoriesAsync(int numberOfStories);
    }
}
