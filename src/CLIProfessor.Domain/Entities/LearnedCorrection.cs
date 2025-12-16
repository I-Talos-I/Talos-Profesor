using Pgvector;

namespace CLIProfessor.Domain.Entities;

public class LearnedCorrection
{
    public Guid Id { get; private set; }
    public string OriginalInput { get; private set; }
    public string CorrectedCommand { get; private set; }
    public string Explanation { get; private set; }
    public Vector? Embedding { get; private set; } // For pgvector
    public DateTime LearnedAt { get; private set; }

    private LearnedCorrection() { } // For EF Core

    public LearnedCorrection(string originalInput, string correctedCommand, string explanation, Vector? embedding = null)
    {
        Id = Guid.NewGuid();
        OriginalInput = originalInput;
        CorrectedCommand = correctedCommand;
        Explanation = explanation;
        Embedding = embedding;
        LearnedAt = DateTime.UtcNow;
    }

    public void UpdateEmbedding(Vector embedding)
    {
        Embedding = embedding;
    }
}
