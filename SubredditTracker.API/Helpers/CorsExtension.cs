namespace SubredditTracker.API.Helpers
{
    public static class CorsExtension
    {
        public static IServiceCollection ConfigureCors(this IServiceCollection service) =>
            service.AddCors(options =>
                        options.AddPolicy("CORSPolicy", policy => policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithOrigins(new[] { "https://localhost:7152" })));
    }
}



