using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CLIProfessor.Domain.Entities;
using CLIProfessor.Domain.Interfaces;
using Pgvector;

namespace CLIProfessor.Application.Features.Learning.Commands;

public class LearnCorrectionHandler : IRequestHandler<LearnCorrectionCommand>
{
    private readonly IGeminiService _geminiService;
    private readonly IVectorStore _vectorStore;

    public LearnCorrectionHandler(IGeminiService geminiService, IVectorStore vectorStore)
    {
        _geminiService = geminiService;
        _vectorStore = vectorStore;
    }

    public async Task Handle(LearnCorrectionCommand request, CancellationToken cancellationToken)
    {
        // 1. Generate embedding for the original input (the "trigger")
        var embeddingFloats = await _geminiService.GenerateEmbeddingAsync(request.OriginalInput);
        
        // Convert float[] to Vector (pgvector)
        var vector = new Vector(embeddingFloats);

        // 2. Create the learned correction entity
        var correction = new LearnedCorrection(
            request.OriginalInput,
            request.CorrectedCommand,
            request.Explanation,
            vector
        );

        // 3. Save to vector store
        await _vectorStore.AddCorrectionAsync(correction);
    }
}
