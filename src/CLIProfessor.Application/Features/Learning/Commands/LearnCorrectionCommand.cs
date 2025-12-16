using MediatR;

namespace CLIProfessor.Application.Features.Learning.Commands;

public record LearnCorrectionCommand(string OriginalInput, string CorrectedCommand, string Explanation) : IRequest;
