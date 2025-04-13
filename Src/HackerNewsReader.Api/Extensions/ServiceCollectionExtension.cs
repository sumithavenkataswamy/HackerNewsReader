using HackerNewsReader.Application.Interfaces;
using HackerNewsReader.Application.Services;
using HackerNewsReader.Infrastructure.Services;

namespace HackerNewsReader.Api.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, ConfigurationManager configuration)
        {
            var hackerNewsApiUrl = configuration.GetValue<string>("HackerNewsApiUrl");
            if (string.IsNullOrEmpty(hackerNewsApiUrl))
            {
                throw new ArgumentException("HackerNewsApiUrl is not configured properly in appsettings.");
            }

            services.AddHttpClient<IHackerNewsReaderService, HackerNewsReaderService>(options =>
            {
                options.BaseAddress = new Uri(hackerNewsApiUrl);
            });

            services.AddMemoryCache();
            services.AddTransient<IStoryService, StoryService>();
            return services;
        }
    }
}
