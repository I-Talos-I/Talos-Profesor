using System.Collections.Generic;
using System.Threading.Tasks;
using CLIProfessor.Domain.Entities;

namespace CLIProfessor.Domain.Interfaces;

public interface IVectorStore
{
    Task AddCorrectionAsync(LearnedCorrection correction);
    Task<IEnumerable<LearnedCorrection>> SearchSimilarAsync(float[] embedding, int limit = 3);
}
