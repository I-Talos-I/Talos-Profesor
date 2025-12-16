using System;

namespace CLIProfessor.Domain.Entities;

public class CommandRequest
{
    public Guid Id { get; private set; }
    public string NaturalLanguageInput { get; private set; }
    public TerminalContext Context { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private CommandRequest() { } // For EF Core

    public CommandRequest(string naturalLanguageInput, TerminalContext context)
    {
        Id = Guid.NewGuid();
        NaturalLanguageInput = naturalLanguageInput;
        Context = context;
        CreatedAt = DateTime.UtcNow;
    }
}
