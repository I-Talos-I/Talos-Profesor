using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using CLIProfessor.Domain.Entities;
using CLIProfessor.Domain.Interfaces;

namespace CLIProfessor.Infrastructure.Persistence;

public class PostgresVectorStore : IVectorStore
{
    private readonly ApplicationDbContext _context;

    public PostgresVectorStore(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddCorrectionAsync(LearnedCorrection correction)
    {
        await _context.LearnedCorrections.AddAsync(correction);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<LearnedCorrection>> SearchSimilarAsync(float[] embedding, int limit = 3)
    {
        if (embedding == null) return Enumerable.Empty<LearnedCorrection>();

        var vector = new Vector(embedding);

        // Using L2 distance for similarity search
        return await _context.LearnedCorrections
            .OrderBy(c => c.Embedding!.L2Distance(vector))
            .Take(limit)
            .ToListAsync();
    }
}
