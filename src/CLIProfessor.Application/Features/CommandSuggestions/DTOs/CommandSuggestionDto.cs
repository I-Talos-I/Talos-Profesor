namespace CLIProfessor.Application.Features.CommandSuggestions.DTOs;

public class CommandSuggestionDto
{
    public string SuggestedCommand { get; set; }
    public string Explanation { get; set; }

    public CommandSuggestionDto(string suggestedCommand, string explanation)
    {
        SuggestedCommand = suggestedCommand;
        Explanation = explanation;
    }
}
