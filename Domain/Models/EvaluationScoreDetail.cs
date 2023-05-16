namespace Domain.Models;

public class EvaluationScoreDetail
{
    public string AgentId { get; set; }
    public string AgentName { get; set; }
    public string AgentIdName
    {
        get
        {
            if (string.IsNullOrEmpty(AgentName))
            {
                return string.Empty;
            }

            var names = AgentName?.Split(' ');

            return names?.Length switch
            {
                > 2 => $"{TrimmedAgentId}-{names[0]} {names[2]}",
                _ => $"{TrimmedAgentId}-{AgentName}"
            };
        }
    }
    public string TrimmedAgentId => string.IsNullOrEmpty(AgentId) ? string.Empty : AgentId.TrimStart('0');
    public string Id { get; set; }
    public string ComponentName { get; set; }
    public string EvaluationId { get; set; }
    public int ComponentValue { get; set; }
    public bool Fufilled { get; set; }
}

