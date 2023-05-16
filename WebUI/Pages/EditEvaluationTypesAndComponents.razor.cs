
namespace WebUI.Pages
{
    public partial class EditEvaluationTypesAndComponents
    {
        [Inject]
        private IEvaluatorProjectUseCases? EvaluatorProjectUseCases { get; set; }
        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationState { get; set; }
        List<Project> projects = new();
        string? loggedInUserId;
        List<string> employeePermissions = new();

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                loggedInUserId = (await AuthenticationState)
                             .User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

                employeePermissions = (await EvaluatorProjectUseCases.GetEmployeePermissionsAsync(loggedInUserId)).ToList();
                await GetProjectByCompany();
                StateHasChanged();
            }
        }

        private async Task GetProjectByCompany()
        {
            if (employeePermissions.Contains(nameof(Permissions.VIEW_SHARED_PROJECTS)))
            {
                var projectsALL = await EvaluatorProjectUseCases.GetProjectsByCompany(new { company = "ALL" });
                projects.AddRange(projectsALL);
            }
            if (employeePermissions.Contains(nameof(Permissions.VIEW_SSS_PROJECTS)))
            {
                var projectsSSS = await EvaluatorProjectUseCases.GetProjectsByCompany(new { company = "SSS" });
                projects.AddRange(projectsSSS);
            }
            if (employeePermissions.Contains(nameof(Permissions.VIEW_SF_PROJECTS)))
            {
                var projectsSF = await EvaluatorProjectUseCases.GetProjectsByCompany(new { company = "SF" });
                projects.AddRange(projectsSF);
            }
        }
    }
}
