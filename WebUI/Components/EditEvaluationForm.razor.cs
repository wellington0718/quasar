namespace WebUI.Components;
public partial class EditEvaluationForm
{
    [CascadingParameter]
    private Error? Error { get; set; }
    [Inject]
    private DialogService? DialogService { get; set; }
    [Inject]
    private NotificationService? NotificationService { get; set; }
    [Inject]
    private IEvaluatorProjectUseCases? EvaluatorProjectUseCases { get; set; }
    [Inject]
    private IEvaluationUseCases? EvaluationUseCases { get; set; }

    [Inject]
    private IEvaluationTypesUseCases? EvaluationTypesUseCases { get; set; }

    private IEnumerable<Employee>? agents = new List<Employee>();
    private IEnumerable<EvaluationType>? evaluationTypes = new List<EvaluationType>();

    [Parameter]
    public EvaluationDTO? SelectedEvaluationDTO { get; set; }

    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationState { get; set; }

    private Employee? selectedAgent;
    private string? loggedInUser;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            loggedInUser = (await AuthenticationState)
              .User.Claims
              .First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            agents = await EvaluatorProjectUseCases.GetEmployeesByProjectAsync(new { SelectedEvaluationDTO?.ProjectId, IncludeEvaluators = false });
            selectedAgent = agents?.FirstOrDefault(agent => agent.Id.Equals(SelectedEvaluationDTO.AgentId));
           var hasSearchByProjectsPermission = (await EvaluatorProjectUseCases.GetEmployeePermissionsAsync(loggedInUser))
               .Any(p => p.Equals(nameof(Permissions.SEARCH_EVALUATIONS_BY_PROJECT)));
            evaluationTypes = await EvaluationTypesUseCases.GetEvaluationTypesAsync(new { SelectedEvaluationDTO.ProjectId, SelectedEvaluationDTO.EvaluatorId, hasSearchByProjectsPermission });
            SelectedEvaluationDTO.Score = 100 - (SelectedEvaluationDTO.EvaluationScoreDetails.Sum(e => e.ComponentValue));
            StateHasChanged();
        }
    }

    private void OnAgentValueChange(Employee employee)
    {
        if (employee is not null)
        {
            SelectedEvaluationDTO.AgentId = employee.Id;
            SelectedEvaluationDTO.AgentName = $"{employee.FirstName} {employee.LastName}";
        }
    }

    private void OnEvaluationTypeValueChange(EvaluationType evaluationType)
    {
        if (evaluationType is not null)
        {
            SelectedEvaluationDTO.EvaluationTypeId = evaluationType.Id;
            SelectedEvaluationDTO.EvaluationTypeName = evaluationType.Name;
        }
    }

    private async Task SaveEvaluationScoreDetail()
    {
        var parameters = new List<dynamic>();

        SelectedEvaluationDTO.EvaluationScoreDetails.ForEach(evaluationScoreDetail =>
        {
            parameters.Add(new {
                evaluationScoreDetail.Id,
                EvaluationId = SelectedEvaluationDTO.Id,
                evaluationScoreDetail.ComponentName,
                evaluationScoreDetail.Fufilled,
                SelectedEvaluationDTO.IsUsingScoreValue,
                evaluationScoreDetail.ComponentValue,
            });
        });
        
        await EvaluationUseCases.SaveEvaluationScoreDetailAsync(parameters);
    }

    private void UpdateScore()
    {
        if(SelectedEvaluationDTO.IsUsingScoreValue)
        {
            var score = (100 - SelectedEvaluationDTO.EvaluationScoreDetails.Sum(e => e.ComponentValue));
            SelectedEvaluationDTO.Score = score < 0 ? 0 : score;
        }
         else
        {
            SelectedEvaluationDTO.Score = SelectedEvaluationDTO.EvaluationScoreDetails.Any(e => !e.Fufilled) ? 0 : 100;
        }
    }
    
    private void OnRadioButtonChanged(bool value, EvaluationScoreDetail evaluationScoreDetailComponent)
    {
        evaluationScoreDetailComponent.Fufilled = value;
        UpdateScore();
    }

    private async Task EditEvaluation()
    {
        var parameters = new
        {
            EvaluationId = SelectedEvaluationDTO.Id,
            SelectedEvaluationDTO.EvaluationTypeId,
            SelectedEvaluationDTO.Score,
            SelectedEvaluationDTO.AgentId,
            EditorId = loggedInUser,
            ModificationDate = DateTime.Now,
            SelectedEvaluationDTO.OriginalDate,
            SelectedEvaluationDTO.EvaluatorId,
            SelectedEvaluationDTO.AccountNumber,
            SelectedEvaluationDTO.CaseNumber,
            SelectedEvaluationDTO.EditionNotes,
            SelectedEvaluationDTO.Comments,
            IsEvaluationCreated = true,
            Enabled = true
        };

        var isSaved = await EvaluationUseCases.SaveEvaluationAsync(parameters);

        if (isSaved)
        {
            await SaveEvaluationScoreDetail();
            SelectedEvaluationDTO.EditionNotes = null;
            SelectedEvaluationDTO.Edited = true;
            DialogService.Close(isSaved);
            NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Evaluation edited", Duration = 2000 });
        }
    }
}