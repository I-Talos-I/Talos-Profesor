using MediatR;
using CLIProfessor.Application.Features.CommandSuggestions.DTOs;
using CLIProfessor.Domain.Entities;

namespace CLIProfessor.Application.Features.CommandSuggestions.Queries;

public class GetCommandSuggestionQuery : IRequest<CommandSuggestionDto>
{
    public string NaturalLanguageInput { get; set; }
    public TerminalContext Context { get; set; }

    public GetCommandSuggestionQuery(string naturalLanguageInput, TerminalContext context)
    {
        NaturalLanguageInput = naturalLanguageInput;
        Context = context;
    }
}
