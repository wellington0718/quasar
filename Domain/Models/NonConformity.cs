namespace Domain.Models;

public class NonConformity
{
    public NonConformity()
    {
        Id = Guid.NewGuid().ToString();
    }

    public string Id { get; }
    public Employee ReportedAgent { get; set; }
    public DateTime IncidentDate { get; set; } = DateTime.Now;
    public DateTime? CloseDate { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime EndDate { get; set; } = DateTime.Now;
    public string RootCauseDesc { get; set; }
    public string ClosedBy { get; set; }
    public Employee Responsible { get; set; }
    public string Reinspection { get; set; }
    public string ResponsibleName { get; set; }
    public string ReportedAgentName { get; set; }
    public string Comments { get; set; }
    public string State { get; set; }
    public bool IsInserting { get; set; } = true;
    public string ApplicationReport { get; set; }
    public string ApplicationType { get; set; } = "NP";
    public string Score { get; set; }
}
