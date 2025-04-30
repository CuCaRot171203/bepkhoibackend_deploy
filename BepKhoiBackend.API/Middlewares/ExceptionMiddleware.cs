using System.Net;
using System.Text.Json;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        // Use to handle exception popular
        int statusCode = exception switch
        {
            KeyNotFoundException => (int)HttpStatusCode.NotFound, // 404 Not Found
            ArgumentException => (int)HttpStatusCode.BadRequest,  // 400 Bad Request
            _ => (int)HttpStatusCode.InternalServerError          // 500 Internal Server Error
        };

        response.StatusCode = statusCode; 

        var result = JsonSerializer.Serialize(new { message = exception.Message });
        return response.WriteAsync(result);
    }
}
