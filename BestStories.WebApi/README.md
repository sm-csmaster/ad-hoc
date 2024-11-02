# BestStories.Retriever

BestStories.Retriever is a .NET 8 application that retrieves and caches the best stories from an external API. 
It uses `HttpClientFactory` for HTTP requests and provides an API to fetch the best stories.

## Features

- Fetches the best stories from an external API.
- Caches story IDs and individual stories to improve performance.
- Handles errors gracefully, logging them and continuing to process other stories.
- Uses `HttpClientFactory` for efficient HTTP client management.
- Provides an API endpoint to retrieve the best stories.

## Prerequisites

- .NET 8 SDK
- Visual Studio or any other IDE that supports .NET development

## Getting Started

### Clone the repository

git clone https://github.com/sm-csmaster/ad-hoc.git cd BestStories.WebApi

### Build and Run

1. Open the solution in Visual Studio BestStories.WebApi\src\BestStories.Retreiver.Service\BestStories.Retriever.Service.sln
2. Restore the NuGet packages.
3. Build the solution.
4. Run the project

### Configuration

The application settings are configured in `appsettings.json` and `launchSettings.json`. Update these files as needed to match your environment.

### API Endpoints

#### Get Best Stories

- **URL**: `GET /api/stories/beststories/{numberOfStories}`
- **Description**: Retrieves the best stories.
- **Parameters**: 
  - `numberOfStories` (int): The number of best stories to retrieve.
- **Response**: A list of stories ordered by score in descending order.

### Example

To fetch the top 10 best stories, make a GET request to:

http://localhost:5192/api/stories/beststories/10

## Project Structure

- **Controllers**: Contains the API controllers.
- **Services**: Contains the service classes that handle the business logic.
- **DTOs**: Contains the data transfer objects.
- **Abstractions**: Contains the interface definitions.
- **Settings**: Contains the configuration settings classes.

## Error Handling

The application logs errors when fetching individual stories fails and continues processing the remaining stories. 
This ensures that a failure in one request does not affect the entire batch.
Additionally, the application uses exception middleware to handle and log unhandled exceptions globally.

## Potential Improvements
While the current implementation is functional, there are several areas where the code could be improved:

### 1. Resilience with Polly
Polly is a .NET library that provides resilience and transient-fault-handling capabilities. 
It can be used to add retry policies, circuit breakers, and other resilience strategies to the `HttpClient` calls. 
This would make the application more robust in the face of transient errors and network issues.

### 2. Rate Limiting
To avoid hitting the external API's rate limits, we can implement rate limiting in the application. 
This can be done using libraries like `AspNetCoreRateLimit` or by implementing custom logic to limit the number 
of requests made to the external API within a certain time frame.

### 3. Optimized Caching Strategy
The current caching strategy stores the entire story object in memory.
This is not efficient when dealing with a large number of stories, a more optimized caching strategy could be implemented.
Also, the cache could be configured to evict old or less frequently accessed stories to free up memory.
Additinally we could use a distributed cache like Redis to store the cached stories.

## License

This project is licensed under the MIT License.