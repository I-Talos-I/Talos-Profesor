using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CLIProfessor.Application.Features.CommandSuggestions.DTOs;
using CLIProfessor.Domain.Interfaces;

namespace CLIProfessor.Application.Features.CommandSuggestions.Queries;

public class GetCommandSuggestionHandler : IRequestHandler<GetCommandSuggestionQuery, CommandSuggestionDto>
{
    private readonly IGeminiService _geminiService;
    private readonly IVectorStore _vectorStore;

    public GetCommandSuggestionHandler(IGeminiService geminiService, IVectorStore vectorStore)
    {
        _geminiService = geminiService;
        _vectorStore = vectorStore;
    }

    public async Task<CommandSuggestionDto> Handle(GetCommandSuggestionQuery request, CancellationToken cancellationToken)
    {
        // 1. Generate embedding for the user input
        var embedding = await _geminiService.GenerateEmbeddingAsync(request.NaturalLanguageInput);

        // 2. Search for similar learned corrections (RAG)
        var similarCorrections = await _vectorStore.SearchSimilarAsync(embedding);

        // 3. Build the prompt
        var promptBuilder = new StringBuilder();
        
        // ROL
        promptBuilder.AppendLine("ROL: Eres un profesor de C# experto en .NET CLI y Arquitectura DDD que enseña a usar la terminal.");
        
        // CONTEXTO
        promptBuilder.AppendLine($"CONTEXTO TÉCNICO: OS: {request.Context.OS}, Shell: {request.Context.Shell}, CWD: {request.Context.CurrentDirectory}");

        // MEMORIA (RAG)
        if (similarCorrections.Any())
        {
            promptBuilder.AppendLine("\nMEMORIA (Correcciones aprendidas anteriormente):");
            foreach (var correction in similarCorrections)
            {
                promptBuilder.AppendLine($"- El usuario preguntó: '{correction.OriginalInput}', tú sugeriste: '{correction.CorrectedCommand}'. Explicación: {correction.Explanation}");
            }
        }

        // INSTRUCCIONES PEDAGÓGICAS
        promptBuilder.AppendLine("\nINSTRUCCIONES PEDAGÓGICAS:");
        promptBuilder.AppendLine("1. FORMATO OBLIGATORIO: La respuesta debe ser SIEMPRE: [CMD: el_comando] [EXP: la_explicación]");
        promptBuilder.AppendLine("2. PEDAGOGÍA:");
        promptBuilder.AppendLine("   - En la explicación ([EXP]), incluye trucos mnemotécnicos para recordar el orden de los comandos (ej: 'Recuerda: Verbo + Plantilla + Nombre').");
        promptBuilder.AppendLine("   - Si el usuario pide crear una solución o proyecto, explica por qué se hace así (ej: 'Primero creamos la solución .sln para agrupar los proyectos').");
        promptBuilder.AppendLine("   - Usa un tono motivador y educativo.");
        promptBuilder.AppendLine("3. NO incluyas markdown, ni bloques de código, solo el formato texto plano especificado.");

        promptBuilder.AppendLine($"\nSOLICITUD DEL ESTUDIANTE: {request.NaturalLanguageInput}");

        // 4. Call Gemini
        var response = await _geminiService.GenerateContentAsync(promptBuilder.ToString());

        // 5. Parse response (Simple parsing for now, assuming Gemini follows instructions)
        // Ideally, we might want a more robust parser or structured output mode.
        var cmdStart = response.IndexOf("[CMD:");
        var cmdEnd = response.IndexOf("]", cmdStart);
        var expStart = response.IndexOf("[EXP:");
        var expEnd = response.IndexOf("]", expStart);

        string command = "echo 'Error parsing response'";
        string explanation = response;

        if (cmdStart != -1 && cmdEnd != -1)
        {
            command = response.Substring(cmdStart + 5, cmdEnd - (cmdStart + 5));
        }

        if (expStart != -1 && expEnd != -1)
        {
            explanation = response.Substring(expStart + 5, expEnd - (expStart + 5));
        }
        else if (cmdEnd != -1)
        {
             // If EXP tag is missing, take the rest of the string or just the raw response if format failed
             // But let's try to be resilient.
             explanation = response.Replace($"[CMD:{command}]", "").Trim();
        }

        return new CommandSuggestionDto(command, explanation);
    }
}
