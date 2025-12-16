using System.Threading.Tasks;

namespace CLIProfessor.Domain.Interfaces;

public interface IGeminiService
{
    Task<string> GenerateContentAsync(string prompt);
    Task<float[]> GenerateEmbeddingAsync(string text);
}
