namespace Domain.Models;

public class EvaluationHistory
{
    public string Id { get; set; }
    public string EvaluationId { get; set; }
    public DateTime ModificationDate { get; set; }
    public DateTime OriginalDate { get; set; }
    public string EditorName { get; set; }
    public string EditorId { get; set; }
    public string EditorNameId => $"{EditorId}-{EditorName}";
    public string ModificationDateString => ModificationDate.ToString("MM/dd/yyyy hh:mm:ss tt");
    public string OriginalDateString => OriginalDate.ToString("MM/dd/yyyy hh:mm:ss tt");
    public string EvaluatorName { get; set; }
    public string EvaluatorNameId => $"{EvaluatorId}-{EvaluatorName}";
    public string EvaluatorId { get; set; }
    public string AgentName { get; set; }
    public string AgentId { get; set; }
    public string AgentNameId => $"{AgentId}-{AgentName}";
    public string CaseNumber { get; set; }
    public int Score { get; set; }
    public string Comments { get; set; }
    public string EditionNotes { get; set; }
}
