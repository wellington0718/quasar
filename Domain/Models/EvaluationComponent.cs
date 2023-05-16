namespace Domain.Models;

public class EvaluationComponent
{
    public EvaluationComponent()
    {
        Id = Guid.NewGuid().ToString();
    }
    public string Id { get; set; }
    public string Name { get; set; }
    public int DeductionValue { get; set; }
    public bool SelectedOption { get; set; } = true;
    public int CurrentDeductionValue { get; set; }
}
