using Microsoft.JSInterop;
using SharedKernel.JsHelpers;

namespace WebUI.Pages
{
    public partial class EditProjectEvaluators
    {
        [CascadingParameter]
        private Error? Error { get; set; }
        [Inject]
        private IEvaluatorProjectUseCases? EvaluatorProjectUseCases { get; set; }
        [Inject]
        private IProjectRepository? ProjectRepository { get; set; }
        [Inject]
        private DialogService? DialogService { get; set; }
        [Inject]
        private NotificationService? NotificationService { get; set; }
        [Inject]
        private IJSRuntime? JSRuntime { get; set; }
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        private List<Project>? projects = new();
        private List<Employee> projectEvaluators = new();
        private List<Employee> projectEmployees = new();
        private List<Employee> allProjectsEmployees = new();
        private List<Employee> moreEmployees = new();
        private List<string> evaluatorIdsToUpdate = new();
        private List<string> employeePermissions = new();
        private List<dynamic> evaluatorsToUpdateParameters = new();
        private string? loggedInUserId;
        private string radzenBody = "radzenBody";
        private EditProjectEvaluationTypeEvaluators? EditProjectEvaluationTypeEvaluators;
        private Project? project = new();

        protected override void OnInitialized()
        {
            DialogService.Open<Loading>(null);
        }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                loggedInUserId = (await authenticationState)
                     .User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

                string? employeeId = null;
                allProjectsEmployees = (await EvaluatorProjectUseCases.GetEmployeesAsync(new { employeeId }))
                    .Where(e => e.Active).ToList();

                moreEmployees = allProjectsEmployees;
                employeePermissions = (await EvaluatorProjectUseCases.GetEmployeePermissionsAsync(loggedInUserId)).ToList();
                await GetProjectByCompany();
                DialogService.Close();
                StateHasChanged();
            }
        }

        private async Task OnSelectedProject()
        {
            if (project is not null)
            {
                DialogService.Open<Loading>(null);
                projectEvaluators = (await EvaluatorProjectUseCases.GetProjectAsignedEvaluatorsAsync(new { projectId = project.Id })).ToList();
                projectEmployees = (await EvaluatorProjectUseCases.GetActiveEmployeesByProjectAsync(new { projectId = project.Id })).ToList();

                moreEmployees = allProjectsEmployees.Where(e => !e.ProjectId.Equals(project.Id)).ToList();
                DialogService.Close();
                return;
            }

            projectEvaluators = new();
            projectEmployees = new();
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

        private void UpdateEvaluatorsIdsList((List<string> Ids, bool IsToAssign) evaluatorsParameters)
        {
            if (project.Id is not null)
            {
                var isToAssign = evaluatorsParameters.IsToAssign;
                var parameters = evaluatorsParameters.Ids.Select(evaluatorId => new
                {
                    projectId = project.Id,
                    isToAssign,
                    evaluatorId
                });

                evaluatorsToUpdateParameters.AddRange(parameters);
            }
        }

        private async Task UpdateEvaluators()
        {
            DialogService.Open<Loading>(null);
            var isSuccess = await EvaluatorProjectUseCases.UpdateProjectEvaluatorsAsync(evaluatorsToUpdateParameters);
            DialogService.Close();

            if (isSuccess)
            {
                evaluatorsToUpdateParameters = new();
                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Evaluators Updated", Duration = 2000 });
            }
        }

    }
}
