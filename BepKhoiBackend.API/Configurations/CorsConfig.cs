namespace BepKhoiBackend.API.Configurations
{
    public static class CorsConfig
    {
        public static void AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            var reactAppUrl = configuration["Cors:ReactAppUrl"];
            Console.WriteLine($"ReactAppUrl loaded: {reactAppUrl}"); 

            services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp",
                    policy =>
                    {
                        policy.WithOrigins(reactAppUrl ?? "http://localhost:3000")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
            });
        }
    }
}
