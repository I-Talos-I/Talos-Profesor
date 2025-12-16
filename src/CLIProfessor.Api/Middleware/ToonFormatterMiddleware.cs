using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using CLIProfessor.Application.Features.CommandSuggestions.DTOs;
using System.Text.Json;

namespace CLIProfessor.Api.Middleware;

public class ToonFormatterMiddleware
{
    private readonly RequestDelegate _next;

    public ToonFormatterMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if the client requested TOON format (e.g. via Accept header or query param, or default for CLI)
        // For this project, we'll assume all responses to /suggest are TOON formatted or we check a header.
        // Let's enforce it for the specific endpoint or globally if it's a dedicated CLI API.
        
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }

        responseBody.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(responseBody).ReadToEndAsync();

        if (context.Response.StatusCode == 200 && context.Request.Path.StartsWithSegments("/suggest"))
        {
            try
            {
                // Try to deserialize the JSON response to CommandSuggestionDto
                // Note: This assumes the endpoint returned a JSON object.
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dto = JsonSerializer.Deserialize<CommandSuggestionDto>(responseText, options);

                if (dto != null)
                {
                    var toonResponse = $"[CMD:{dto.SuggestedCommand}] [EXP:{dto.Explanation}]";
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync(toonResponse);
                    return;
                }
            }
            catch
            {
                // If deserialization fails, just return original (or handle error)
            }
        }

        // If not handled, write back the original response
        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
    }
}
