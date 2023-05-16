namespace WebUI.Components;

public partial class EditProjectEvaluationTypeEvaluators
{
    [CascadingParameter]
    private Error? Error { get; set; }
    [Parameter]
    public List<Employee> ProjectEvaluationTypeEvaluators { get; set; } = new();
    [Parameter]
    public List<Employee> ProjectEmployeesEvaluators { get; set; } = new();
    [Parameter]
    public List<Employee> MoreProjectsEmployees { get; set; } = new();
    [Parameter]
    public string? EvaluatorsTableTitle { get; set; }
    [Parameter]
    public string? ProjectEmployeesTableTitle { get; set; }
    [Parameter]
    public EventCallback<(List<string> Ids, bool IsToAssign)> UpdateEvaluatorsIdsList { get; set; }

    private bool evaluatorsTableRowsSelected;
    private bool moreEmployeesTableRowsSelected;
    private bool projectTableRowsSelected;
    private EmployeesTable? projectEmployeesTable;
    private EmployeesTable? moreEmployeesTable;
    private EmployeesTable? evaluatorsTable;

    private async Task SwapProjectEvaluators()
    {
        await RemoveProjectEvaluators();
        await AddProjectEvaluators();

        projectTableRowsSelected = false;
        evaluatorsTableRowsSelected = false;
        moreEmployeesTableRowsSelected = false;
        await projectEmployeesTable.EmployeesGrid.Reload();
        await evaluatorsTable.EmployeesGrid.Reload();
    }

    private async Task RemoveProjectEvaluators()
    {
        if (evaluatorsTable.SelectedEmployees?.Any() ?? false)
        {
            var selectedProjectEvaluators = evaluatorsTable.SelectedEmployees.ToList();
            ProjectEvaluationTypeEvaluators.RemoveAll(projectEvaluator => selectedProjectEvaluators.Contains(projectEvaluator));
            await UpdateEvaluatorsIdsList.InvokeAsync((selectedProjectEvaluators.Select(e => e.Id).ToList(), false));
            evaluatorsTable.SelectedEmployees = null;
        }
    }

    private async Task AddProjectEvaluators()
    {
        var selectedProjectEmployees = new List<Employee>();
        var projectEmployees = projectEmployeesTable?.SelectedEmployees?? new List<Employee>();
        var otherProjectsEmployees = moreEmployeesTable?.SelectedEmployees?? new List<Employee>();

        selectedProjectEmployees.AddRange(projectEmployees);
        selectedProjectEmployees.AddRange(otherProjectsEmployees);

        selectedProjectEmployees.RemoveAll(selectedProjectEmployee => ProjectEvaluationTypeEvaluators
               .Any(projectEvaluator => projectEvaluator.Id.Equals(selectedProjectEmployee.Id)));

        if (selectedProjectEmployees.Any())
        {
            ProjectEvaluationTypeEvaluators.AddRange(selectedProjectEmployees);
            await UpdateEvaluatorsIdsList.InvokeAsync((selectedProjectEmployees.Select(e => e.Id).ToList(), true));
        }

        if (moreEmployeesTable is not null)
            moreEmployeesTable.SelectedEmployees = null;

        projectEmployeesTable.SelectedEmployees = null;
    }

    private void HandleProjectTableRowSelectedValueChanged(bool rowSelected)
    {
        projectTableRowsSelected = rowSelected;
    }

    private void HandleEvaluatorTableRowSelectedValueChanged(bool rowSelected)
    {
        evaluatorsTableRowsSelected = rowSelected;
    }

    private void HandleMoreEmployeesTableRowSelectedValueChanged(bool rowSelected)
    {
        moreEmployeesTableRowsSelected = rowSelected;
    }
}

