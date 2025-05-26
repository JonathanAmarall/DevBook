namespace Domain.Services;
public interface ITextSummaryGeneratorService
{
    Task<string> GenerateTextSummaryAsync(string text, CancellationToken cancellationToken);
}
