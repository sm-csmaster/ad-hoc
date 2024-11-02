using BestStories.Retriever.Application.Abstractions;
using BestStories.Retriever.Application.Services;
using BestStories.Retriever.Application.Settings;
using BestStories.Retriever.Service.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<ExternalApiSettings>(builder.Configuration.GetSection("ExternalAPI"));
builder.Services.AddHttpClient<StoryService>();
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddScoped<IStoryService, StoryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
