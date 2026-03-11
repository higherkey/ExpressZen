namespace ExpressZen.Client.Models;

public class TestData
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Text { get; set; } = string.Empty;
    public bool ExpectedToMatch { get; set; }
    public RegexResult? Result { get; set; }
}
