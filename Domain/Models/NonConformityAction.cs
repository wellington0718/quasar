namespace Domain.Models;

public class NonConformityAction
{
    public NonConformityAction()
    {
        Id = Guid.NewGuid().ToString();
    }

    public string Id { get; set; }
    public string NonConformityId { get; set; }
    public string Description { get; set; }
    public string ResponsibleId { get; set; }
    public string ResponsibleName { get; set; }
    public Employee Responsible { get; set; }
    public string Verification { get; set; }
    public string Type { get; set; }
    public string State { get; set; } = "In Process";
    public DateTime? CloseDate { get; set; }
    public DateTime ExpectedDate { get; set; } = DateTime.Now;
    public DateTime? ComplianceDate { get; set; }
    public bool IsInserting { get; set; } = true;
}
