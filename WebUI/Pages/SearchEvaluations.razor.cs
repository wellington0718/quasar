namespace WebUI.Pages;

public partial class SearchEvaluations
{
    [Inject]
    private DialogService? DialogService { get; set; }
    [Inject]
    private IEvaluationUseCases? EvaluationUseCases { get; set; }
    [Inject]
    private IEvaluatorProjectUseCases? EvaluatorProjectUseCases { get; set; }
    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationState { get; set; }
    [CascadingParameter]
    private Error? Error { get; set; }
    private List<EvaluationDTO>? EvaluationDTOs;
    private EvaluationDTO model = new();
    private string? userId;
    private bool hasSearchByProjectsPermission;
    private bool isEvaluator;

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            DialogService.Open<Loading>(null);
            userId = (await AuthenticationState)
                   .User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

            hasSearchByProjectsPermission = (await EvaluatorProjectUseCases.GetEmployeePermissionsAsync(userId))
                .Any(p => p.Equals(nameof(Permissions.SEARCH_EVALUATIONS_BY_PROJECT)));

            isEvaluator = await EvaluatorProjectUseCases.IsEmployeeEvaluatorAsync(new { EmployeeId = userId });

            DialogService.Close();
            StateHasChanged();
        }
    }

    private async Task HandleSearchEvaluations()
    {
        if (model.DateTimeRange.IsDateRangeInValid()) return;

        DialogService?.Open<Loading>(null);
        var agentId = (hasSearchByProjectsPermission || isEvaluator) ? model.AgentId : userId;
        var loggedInUserId = hasSearchByProjectsPermission ? model.EvaluatorId : userId;

        var projectId = string.Empty;
        if (string.IsNullOrEmpty(model.ProjectId) && model.Projects is not null)
        {
            var projectIdIds = model.Projects.Select(p => p.Id).ToList();
            projectId = string.Join(",", projectIdIds);
        }
        else
        {
            projectId = model.ProjectId;
        }

        var parameters = new
        {
            OriginalStartDate = model.DateTimeRange.Start,
            OriginalEndDate = model.DateTimeRange.End,
            agentId,
            model.AgentName,
            UserId = loggedInUserId,
            projectId,
            model.EvaluatorId,
            model.EvaluatorName,
            model.CaseNumber,
            model.State,
            model.AccountNumber,
            model.EvaluationTypeId,
            IsErrorsOnly = false
        };

        EvaluationDTOs = (await EvaluationUseCases.SearchEvaluationsAsync(parameters)).ToList();
        DialogService?.Close(false);
    }

}

