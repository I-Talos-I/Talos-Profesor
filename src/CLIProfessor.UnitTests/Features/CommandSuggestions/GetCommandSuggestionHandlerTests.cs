using System.Threading;
using System.Threading.Tasks;
using CLIProfessor.Application.Features.CommandSuggestions.DTOs;
using CLIProfessor.Application.Features.CommandSuggestions.Queries;
using CLIProfessor.Domain.Entities;
using CLIProfessor.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CLIProfessor.UnitTests.Features.CommandSuggestions;

public class GetCommandSuggestionHandlerTests
{
    private readonly Mock<IGeminiService> _geminiServiceMock;
    private readonly Mock<IVectorStore> _vectorStoreMock;
    private readonly GetCommandSuggestionHandler _handler;

    public GetCommandSuggestionHandlerTests()
    {
        _geminiServiceMock = new Mock<IGeminiService>();
        _vectorStoreMock = new Mock<IVectorStore>();
        _handler = new GetCommandSuggestionHandler(_geminiServiceMock.Object, _vectorStoreMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuggestion_WhenGeminiReturnsValidResponse()
    {
        // Arrange
        var request = new GetCommandSuggestionQuery("listar archivos", new TerminalContext("Linux", "Bash", "/home/user"));
        var embedding = new float[] { 0.1f, 0.2f };
        var geminiResponse = "[CMD:ls -la] [EXP:List all files including hidden ones.]";

        _geminiServiceMock.Setup(x => x.GenerateEmbeddingAsync(It.IsAny<string>()))
            .ReturnsAsync(embedding);

        _vectorStoreMock.Setup(x => x.SearchSimilarAsync(embedding, 3))
            .ReturnsAsync(new List<LearnedCorrection>());

        _geminiServiceMock.Setup(x => x.GenerateContentAsync(It.IsAny<string>()))
            .ReturnsAsync(geminiResponse);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.SuggestedCommand.Should().Be("ls -la");
        result.Explanation.Should().Be("List all files including hidden ones.");
    }

    [Fact]
    public async Task Handle_ShouldIncludeMemory_WhenSimilarCorrectionsExist()
    {
        // Arrange
        var request = new GetCommandSuggestionQuery("listar archivos", new TerminalContext("Linux", "Bash", "/home/user"));
        var embedding = new float[] { 0.1f, 0.2f };
        var pastCorrection = new LearnedCorrection("listar", "ls", "List files");

        _geminiServiceMock.Setup(x => x.GenerateEmbeddingAsync(It.IsAny<string>()))
            .ReturnsAsync(embedding);

        _vectorStoreMock.Setup(x => x.SearchSimilarAsync(embedding, 3))
            .ReturnsAsync(new List<LearnedCorrection> { pastCorrection });

        _geminiServiceMock.Setup(x => x.GenerateContentAsync(It.Is<string>(s => s.Contains("MEMORIA"))))
            .ReturnsAsync("[CMD:ls] [EXP:List files]");

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _geminiServiceMock.Verify(x => x.GenerateContentAsync(It.Is<string>(s => s.Contains("MEMORIA"))), Times.Once);
    }
}
