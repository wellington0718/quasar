namespace Domain.Models;

public class EvaluationType
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool Enabled { get; set; }
    public string ProjectId { get; set; }
    public string Project { get; set; }
    public bool IsInclusive { get; set; }
    public bool IsUsingScoreValue { get; set; }
}
