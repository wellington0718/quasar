namespace Domain.Models;

public class Evaluation
{
    public string Id { get; set; }
    public DateTime OriginalDate { get; set; }
    public DateTime ModificationDate { get; set; }
    public string EvaluationTypeId { get; set; }
    public string AgentId { get; set; }
    public string EvaluatorId { get; set; }
    public string CaseNumber { get; set; }
    public int Score { get; set; }
    public string Comments { get; set; }
    public string AccountNumber { get; set; }
    public bool Edited { get; set; }
    public bool Enabled { get; set; }
}
