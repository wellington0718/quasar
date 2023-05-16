using Radzen.Blazor;

namespace WebUI.Components;

public partial class EmployeesTable
{
    [CascadingParameter]
    private Error? Error { get; set; }
    [Parameter]
    public List<Employee> Employees { get; set; } = new();
    [Parameter]
    public string? ColumnTitle { get; set; }
    [Parameter]
    public EventCallback<bool> OnRowSelectedValueChanged { get; set; }

    private bool? checkAllState = false;
    public RadzenDataGrid<Employee>? EmployeesGrid { get; set; }
    public IList<Employee>? SelectedEmployees { get; set; }
    private string? pagingSummary;
    protected override void OnParametersSet()
    {
        int selectedEmployeesCount = SelectedEmployees?.Count ?? 0;
        int projectEmployeesCount = Employees?.Count ?? 0;

        var currentPage = Employees.Count > 0 ? 1 : 0;
        pagingSummary = $"Displaying page {currentPage} of {Math.Ceiling(Employees.Count/10m)} (total {Employees.Count()} records)";
        if (projectEmployeesCount > 0)
        {
            checkAllState = (

               projectEmployeesCount == selectedEmployeesCount
               ? true
               : selectedEmployeesCount == 0
               ? false
               : null
           );
        }
        else
        {
            checkAllState = false;
        }
    }

    private void OnPage(PagerEventArgs args)
    {
        pagingSummary = $"Displaying page {args.PageIndex + 1} of {Math.Ceiling(Employees.Count/10m)} (total {Employees.Count()} records)";
    }

    private async Task HandleRowSelectedValueChanged(IList<Employee> SelectedEmployeesItems)
    {
        SelectedEmployees = SelectedEmployeesItems;
        await OnRowSelectedValueChanged.InvokeAsync(SelectedEmployeesItems?.Any() ?? false);
    }

    private async Task HandleCheckAllStateChanged()
    {
        checkAllState =  !(checkAllState?? false);
        SelectedEmployees = checkAllState == true ? Employees?.ToList() : new();
        await OnRowSelectedValueChanged.InvokeAsync(SelectedEmployees?.Any() ?? false);
    }
}

