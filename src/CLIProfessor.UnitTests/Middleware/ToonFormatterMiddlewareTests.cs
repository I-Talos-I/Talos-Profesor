using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CLIProfessor.Api.Middleware;
using CLIProfessor.Application.Features.CommandSuggestions.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace CLIProfessor.UnitTests.Middleware;

public class ToonFormatterMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldFormatResponse_WhenPathIsSuggestAndStatusIs200()
    {
        // Arrange
        var middleware = new ToonFormatterMiddleware(async (innerContext) =>
        {
            // Simulate Controller Response
            innerContext.Response.StatusCode = 200;
            innerContext.Response.ContentType = "application/json";
            var dto = new CommandSuggestionDto("ls", "List files");
            await innerContext.Response.WriteAsync(JsonSerializer.Serialize(dto));
        });

        var context = new DefaultHttpContext();
        context.Request.Path = "/suggest";
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();

        responseText.Should().Be("[CMD:ls] [EXP:List files]");
    }

    [Fact]
    public async Task InvokeAsync_ShouldPassThrough_WhenPathIsNotSuggest()
    {
        // Arrange
        var originalResponse = "Some other content";
        var middleware = new ToonFormatterMiddleware(async (innerContext) =>
        {
            await innerContext.Response.WriteAsync(originalResponse);
        });

        var context = new DefaultHttpContext();
        context.Request.Path = "/health";
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();

        responseText.Should().Be(originalResponse);
    }
}
