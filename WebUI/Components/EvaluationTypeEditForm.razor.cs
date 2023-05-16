using Radzen.Blazor;

namespace WebUI.Components;

public partial class EvaluationTypeEditForm
{
    [CascadingParameter]
    private Error? Error { get; set; }
    [Inject]
    private IEvaluatorProjectUseCases? EvaluatorProjectUseCases { get; set; }
    [Inject]
    private IEvaluationTypesUseCases? EvaluationTypesUseCases { get; set; }
    [Inject]
    private DialogService? DialogService { get; set; }
    [Inject]
    private NotificationService? NotificationService { get; set; }

    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationState { get; set; }
    private string? loggedInUserId;
    private bool hasAnyComponents = true;
    private List<Project> projects = new();
    private List<EvaluationComponent> components = new();
    private List<string> employeePermissions = new();
    private List<dynamic> evaluatorsToUpdateParameters = new();
    private List<Employee> projectEmployeesEvaluators = new();
    private List<Employee> projectEvaluationTypeEvaluators = new();
    private List<Employee> moreEmployees = new();
    private List<EvaluationType> evaluationTypes = new();
    private CreateEvaluationTypesComponents? createEvaluationTypesComponents;
    public CreateEvaluationTypeDTO model = new();
    private EditProjectEvaluationTypeEvaluators? EditProjectEvaluationTypeEvaluators;
    private RadzenDropDownDataGrid<EvaluationType>? EvaluationTypeDropDown;

    [Parameter]
    public bool IsEditingType { get; set; }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            DialogService.Open<Loading>(null);
            loggedInUserId = (await AuthenticationState)
                 .User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

            employeePermissions = (await EvaluatorProjectUseCases.GetEmployeePermissionsAsync(loggedInUserId)).ToList();

            string? employeeId = null;
            moreEmployees = (await EvaluatorProjectUseCases.GetEmployeesAsync(new { employeeId }))
                   .Where(e => e.Active).ToList();

            await GetProjectByCompany();
            if (!IsEditingType)
            {
                model.IsInclusive = true;
                model.IsUsingScoreValue = true;
            }

            StateHasChanged();
            DialogService.Close();
        }
    }

    private async Task GetProjectByCompany()
    {
        var companyProjects = new List<Project>();
        if (employeePermissions.Contains(nameof(Permissions.VIEW_SHARED_PROJECTS)))
        {
            var projectsALL = (await EvaluatorProjectUseCases.GetProjectsByCompany(new { company = "ALL" })).ToList();
            companyProjects.AddRange(projectsALL);
        }
        if (employeePermissions.Contains(nameof(Permissions.VIEW_SSS_PROJECTS)))
        {
            var projectsSSS = (await EvaluatorProjectUseCases.GetProjectsByCompany(new { company = "SSS" })).ToList();
            companyProjects.AddRange(projectsSSS);
        }
        if (employeePermissions.Contains(nameof(Permissions.VIEW_SF_PROJECTS)))
        {
            var projectsSF = (await EvaluatorProjectUseCases.GetProjectsByCompany(new { company = "SF" })).ToList();
            companyProjects.AddRange(projectsSF);
        }

        projects = companyProjects;
    }

    private async Task OnProjectSelect(object project)
    {
        model.Project = ((Project)project);

        if (IsEditingType)
        {
            model.EvaluationType  = null;
        }

        if (project is not null)
        {
            var evaluatorId = string.Empty;
            moreEmployees = moreEmployees.Where(e => !e.ProjectId.Equals(model.Project.Id)).ToList();
            projectEmployeesEvaluators = (await EvaluatorProjectUseCases.GetProjectAsignedEvaluatorsAsync(new { projectId = model.Project.Id })).ToList();
            evaluationTypes = (await EvaluationTypesUseCases.GetEvaluationTypesAsync(new { ProjectId = model.Project.Id, HasSearchByProjectsPermission = true, evaluatorId })).ToList();
        }

        ResetForm();
    }

    private void OnTypeNameChange(string typeName)
    {
        model.Name = typeName;
    }

    private async Task OnEvaluationTypeSelect(object evaluationTypeObj)
    {
        var evaluationType = ((EvaluationType)evaluationTypeObj);

        if (evaluationType is not null)
        {
            DialogService.Open<Loading>(null);
            components = (await EvaluationTypesUseCases.GetEvaluationTypeComponentsAsync(new { evaluationType.IsUsingScoreValue, EvaluationTypeId = model.EvaluationType.Id })).ToList();
            hasAnyComponents = components.Any();
            model.Name = evaluationType.Name;
            model.Enabled = evaluationType.Enabled;
            model.EvaluationTypeId = evaluationType.Id;
            model.IsUsingScoreValue = evaluationType.IsUsingScoreValue;
            model.IsInclusive = evaluationType.IsInclusive;

            projectEvaluationTypeEvaluators = (await EvaluationTypesUseCases.GetEvaluationTypeEvaluatorsAsync(new { EvaluationTypeId = model.EvaluationType.Id })).ToList();

            DialogService.Close();
            StateHasChanged();
        }
    }

    private async void SaveEvaluationType()
    {
        if (!model.Enabled && IsEditingType)
        {
            var message = "Remember that when disabling, the exclusive evaluators will be removed from this evaluation type. Also, evaluators for the project will not be able to use this evaluation type. Are you sure you want to keep this evaluation type disabled?";
            var response = await DialogService.Confirm(message, "Disabling evaluation type",
            new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No", CloseDialogOnOverlayClick = true, Style="min-width:800px !important" });

            if (!(response?? false)) return;
        }

        var enabled = model.EvaluationType is not null ? model.Enabled : true;
        var evaParameters = new
        {
            model.EvaluationTypeId,
            ProjectId = model.Project.Id,
            model.Name,
            model.IsInclusive,
            IsUsingScoreValue = model.IsUsingScoreValue,
            enabled
        };

        DialogService.Open<Loading>(null);
        hasAnyComponents = createEvaluationTypesComponents?.Components?.Any() ?? false;

        if (!hasAnyComponents)
        {
            DialogService.Close();
            return;
        };

        var success = await EvaluationTypesUseCases.SaveEvaluationTypeAsync(evaParameters);
        var componentParameters = new List<dynamic>();

        if (success)
        {
            if (!evaluationTypes.Any(e => e.Id.Equals(model.EvaluationTypeId)))
            {
                var type = new EvaluationType
                {
                    Name = model.Name,
                    ProjectId = model.Project.Id,
                    Id = model.EvaluationTypeId,
                    IsInclusive = model.IsInclusive,
                    Enabled = true
                };

                evaluationTypes.Add(type);
            }

            UpdateEvaluationTypeDropdownWhenEdited();

            await UpdateEvaluationTypeEvaluators();
            var componentsSavedSuccess = await SaveEvaluationsConponents();

            if (componentsSavedSuccess)
            {
                var summery = IsEditingType ? "Evaluation type updated" : "Evaluation type created";
                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = summery, Duration = 2000 });
                DialogService.Close();

                if (!IsEditingType)
                {

                    ResetForm();
                }
            }
        }
    }

    private void UpdateEvaluationTypeDropdownWhenEdited()
    {
        if (model.EvaluationType is not null)
        {
            model.EvaluationType.Name = model.Name;
            StateHasChanged();
        }
    }

    private async Task<bool> SaveEvaluationsConponents()
    {
        var evaluationTypeComponentsFromDB = (await EvaluationTypesUseCases.GetEvaluationTypeComponentsAsync(new { model.IsUsingScoreValue, model.EvaluationTypeId })).ToList();
        components = createEvaluationTypesComponents.Components;

        var componentParameters = new List<dynamic>();

        foreach (var component in components)
        {
            componentParameters.Add(new
            {
                model.EvaluationTypeId,
                component.Name,
                component.Id,
                component.DeductionValue,
                model.IsUsingScoreValue,
                State = "Add"
            });
        }

        var componentsToDelete = evaluationTypeComponentsFromDB.Where(e => !componentParameters.Any(c => c.Id.Equals(e.Id)));

        foreach (var componentToDelete in componentsToDelete)
        {
            componentParameters.Add(new
            {
                model.EvaluationTypeId,
                componentToDelete.Name,
                componentToDelete.Id,
                componentToDelete.DeductionValue,
                model.IsUsingScoreValue,
                State = "Delete"
            });
        }

        var result = await EvaluationTypesUseCases.SaveEvaluationTypeComponentAsync(componentParameters);

        return result;
    }

    private void ResetForm()
    {
        model.EvaluationTypeId = Guid.NewGuid().ToString();
        model.Name = model.EvaluationType is not null ? model.Name : string.Empty;
        components = model.EvaluationType is not null ? components : new();
        projectEvaluationTypeEvaluators = new();

        if (!IsEditingType)
            model.IsInclusive = true;

        StateHasChanged();
    }

    private void UpdateEvaluatorsIdsList((List<string> Ids, bool IsToAssign) evaluatorsParameters)
    {
        var isToAssign = evaluatorsParameters.IsToAssign;
        var parameters = evaluatorsParameters.Ids.Select(evaluatorId => new
        {
            model.EvaluationTypeId,
            isToAssign,
            evaluatorId
        });

        evaluatorsToUpdateParameters.AddRange(parameters);
    }

    private async Task UpdateEvaluationTypeEvaluators()
    {
        if (model.IsInclusive)
        {
            evaluatorsToUpdateParameters = new();

            var parameters = projectEvaluationTypeEvaluators.Select(p => new
            {
                isToAssign = false,
                model.EvaluationTypeId,
                p.Id
            });

            evaluatorsToUpdateParameters.AddRange(parameters);
            projectEvaluationTypeEvaluators = new();
        };

        await EvaluationTypesUseCases.UpdateEvaluationTypeEvaluators(evaluatorsToUpdateParameters);
        evaluatorsToUpdateParameters = new();
    }
}

