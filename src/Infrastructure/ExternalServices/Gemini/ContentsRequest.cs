namespace Infrastructure.ExternalServices.Gemini;
public record ContentRequest
{
    public List<PartRequest> Parts { get; init; }
}

public record PartRequest
{
    public string Text { get; set; }

    public void AddText(string text)
    {
        Text = text;
    }
}

public record ContentsRequest
{
    public List<ContentRequest> Contents { get; init; }

    public void AddContent(ContentRequest content)
    {
        Contents.Add(content);
    }
}
