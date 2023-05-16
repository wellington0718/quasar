using Domain.Models;
using Radzen.Blazor;

namespace WebUI.Pages;

public partial class EvaluateAgent
{
    [Inject]
    private DialogService? DialogService { get; set; }
    [Inject]
    private NotificationService? NotificationService { get; set; }

    [CascadingParameter]
    private Error? Error { get; set; }

    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationStateTask { get; set; }
    [Inject]
    private IEvaluationUseCases? EvaluationUseCases { get; set; }
    [Inject]
    private IEvaluationTypesUseCases? EvaluationTypesUseCases { get; set; }
    [Inject]
    private IEvaluatorProjectUseCases? EvaluatorProjectUseCases { get; set; }

    private EvaluationDTO model = new();
    private DateTime evaluationDate = DateTime.Today;
    private List<EvaluationDTO> currentDayEvaluations = new();
    private List<EvaluationComponent> evaluationComponents = new();
    private int evaluationsByMonthCount;
    private float averageScore;
    private int evaluationsByDayCount;

    private EvaluationsTable? EvaluationsTableRef;
    private RadzenDropDownDataGrid<EvaluationType>? EvaluationTypesGridRef;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            DialogService.Open<Loading>(null);
            try
            {
                model.EvaluatorId = (await AuthenticationStateTask)
               .User.Claims
               .First(c => c.Type == ClaimTypes.NameIdentifier).Value;

                model.Projects = (await EvaluatorProjectUseCases.GetEvaluatorAssignedProjectsAsync(new { EvaluatorId = model.EvaluatorId })).ToList();
                if(model.Projects.Count == 1)
                {
                    model.ProjectId = model.Projects[0].Id;
                   await GetEvaluationTypesAndProjectEmployess(model.ProjectId);
                }
                currentDayEvaluations = (await EvaluationUseCases.GetCurrentDayEvaluationsAsync(new { model.EvaluatorId })).ToList();
                DialogService.Close();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Error.ProcessError(ex);
            }
        }
    }

    private async Task GetEvaluationTypesAndProjectEmployess(string projectId)
    {
        model.EvaluationTypes = (await EvaluationTypesUseCases.GetEvaluationTypesAsync(new { model.EvaluatorId, projectId, hasSearchByProjectsPermission = false })).ToList();
        model.ProjectEmployees = (await EvaluatorProjectUseCases.GetEmployeesByProjectAsync(new { projectId, IncludeEvaluators = false }))
             .Where(employee => employee.Active).ToList(); 
    }

    private async Task OnSelectedProject()
    {
        if (model.ProjectId is not null)
        {
            await GetEvaluationTypesAndProjectEmployess(model.ProjectId);
        }else
        {
            model.EvaluationTypes = new List<EvaluationType>();
        }

        EvaluationTypesGridRef.Reset();
    }

    private void OnRadioButtonChanged(bool value, EvaluationComponent evaluationComponent)
    {
        evaluationComponent.SelectedOption = value;
        CalculateScore(evaluationComponent.DeductionValue, evaluationComponent);
    }

    private void CalculateScore(int deductionValue, EvaluationComponent component)
    {
        if (model.IsUsingScoreValue)
        {
            if (deductionValue < 0) return;
            int score = 100;

            component.CurrentDeductionValue = component.CurrentDeductionValue > 0
                        ? component.CurrentDeductionValue
                        : deductionValue;

            if (!component.SelectedOption)
            {
                score = (score - evaluationComponents.Sum(component => component.CurrentDeductionValue));

                model.Score = score < 0 ? 0 : score;
            }
            else
            {
                score = model.Score;

                score += evaluationComponents
                    .Where(component => component.SelectedOption)
                    .Sum(component => component.CurrentDeductionValue);

                model.Score = score > 100 ? 100 : score;
                component.CurrentDeductionValue = 0;
            }
        }
        else
        {
            model.Score = evaluationComponents.Any(e => !e.SelectedOption) ? 0 : 100;
        }


    }
    private void CalculateAgentStats()
    {
        if (model.Agent is not null && model.EvaluationTypeId is not null)
        {
            DialogService.Open<Loading>(null);

            //var parameters = new { AgentId = model.Agent.Id, model.EvaluationTypeId };

            //var stat = await EvaluationUseCases.GetEvaluationsAVGByMonthAsync(parameters);
            //averageScore = stat.Item1;
            //evaluationsByMonthCount = stat.Item2;
            evaluationsByDayCount = currentDayEvaluations.Where(eva => eva.AgentId.Equals(model.Agent.Id)
            && eva.EvaluationTypeId.Equals(model.EvaluationTypeId)).Count();
            DialogService.Close();

            StateHasChanged();
        }
    }

    private async Task SaveEvaluation()
    {
        DialogService.Open<Loading>(null);

        var evaluator = (await EvaluatorProjectUseCases.GetEmployeesAsync(new { EmployeeId = model.EvaluatorId })).FirstOrDefault();
        model.Id = Guid.NewGuid().ToString();
        var modificationDate = DateTime.Now;

        var parameters = new
        {
            EvaluationId = model.Id,
            model.EvaluationTypeId,
            AgentId = model.Agent.Id,
            EditorId = model.EvaluatorId,
            EditionNotes = "Evaluation was created.",
            model.Score,
            modificationDate,
            OriginalDate = evaluationDate,
            model.EvaluatorId,
            model.CaseNumber,
            model.AccountNumber,
            Enabled = true,
            IsEvaluationCreated = false,
            model.Comments
        };

        var isSuccess = await EvaluationUseCases.SaveEvaluationAsync(parameters);

        if (isSuccess)
        {
            await SaveEvaluationScoreDetail();
            await UpdateAgentStats(null);
            ResetEvaluationForm();
            NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Evaluation created", Duration = 2000 });
        }

        DialogService.Close();
    }

    private async Task UpdateAgentStats(string? evaluationId)
    {
        currentDayEvaluations = (await EvaluationUseCases.GetCurrentDayEvaluationsAsync(new { model.EvaluatorId })).ToList();

        var agentEvaluationsByDay = (currentDayEvaluations.Where(evaluation =>
                                   evaluation.AgentId.Equals(model.Agent.Id)
                                   && evaluation.EvaluationTypeId.Equals(model.EvaluationTypeId))).ToList();

        if (agentEvaluationsByDay.Any())
        {
            averageScore = (float)agentEvaluationsByDay.Average(e => e.Score);
            evaluationsByDayCount = agentEvaluationsByDay.Count();
            evaluationsByMonthCount += 1;
        }
        else
        {
            evaluationsByDayCount = 0;
            averageScore = 0;
            evaluationsByMonthCount= 0;
        }

        await EvaluationsTableRef.evaluationsGrid.Reload();
    }

    private async Task SaveEvaluationScoreDetail()
    {
        var parameters = evaluationComponents?.Select(component => new
        {
            Id = Guid.NewGuid().ToString(),
            EvaluationId = model.Id,
            ComponentName = component.Name,
            ComponentValue = component.CurrentDeductionValue,
            model.IsUsingScoreValue,
            Fufilled = component.SelectedOption
        }); ;

        await EvaluationUseCases.SaveEvaluationScoreDetailAsync(parameters);
    }

    private void OnSelectedAgent()
    {
        if (model.Agent is not null)
        {
            ResetEvaluationForm();
            CalculateAgentStats();
        }
    }

    private async void OnEvaluationTypeChange(EvaluationType evaluationType)
    {
        if (evaluationType is not null)
        {
            evaluationComponents = (await EvaluationTypesUseCases.GetEvaluationTypeComponentsAsync(new { evaluationType.IsUsingScoreValue, evaluationTypeId = evaluationType.Id })).ToList();
            model.EvaluationTypeName = evaluationType.Name;
            model.EvaluationTypeId = evaluationType.Id;
            model.IsUsingScoreValue = evaluationType.IsUsingScoreValue;
            CalculateAgentStats();
            ResetEvaluationForm();
            StateHasChanged();
            return;
        }
        model.Agent = null;
        model.EvaluationTypeId =  null;
    }

    private void ResetEvaluationForm()
    {
        evaluationComponents.ForEach(component =>
        {
            model.Score = 100;
            model.CaseNumber = string.Empty;
            model.AccountNumber = string.Empty;
            component.SelectedOption = true;
            component.CurrentDeductionValue = 0;
            model.Comments = "Great.";
            evaluationDate = DateTime.Now.Date;

        });
    }
}
