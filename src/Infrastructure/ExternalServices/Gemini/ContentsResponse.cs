namespace Infrastructure.ExternalServices.Gemini;

public record ContentsResponse
{
    public List<CandidateResponse> Candidates { get; init; }
    public string ModelVersion { get; init; }
    public string ResponseId { get; init; }
}

public record CandidateResponse
{
    public ContentResponse Content { get; init; }
    public string FinishReason { get; init; }
    public double AvgLogprobs { get; init; }
}

public record ContentResponse
{
    public List<PartResponse> Parts { get; init; }
    public string Role { get; init; }
}

public record PartResponse
{
    public string Text { get; init; }
}



