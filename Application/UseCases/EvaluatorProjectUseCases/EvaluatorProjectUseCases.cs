using Microsoft.Extensions.Hosting;
using SharedKernel.Interfaces;

namespace Application.UseCases.EvaluatorProjectUseCases
{
    public class EvaluatorProjectUseCases : IEvaluatorProjectUseCases
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IProjectRepository projectRepository;
        private readonly IApplicationService applicationService;

        public EvaluatorProjectUseCases(IApplicationService applicationService, IEmployeeRepository employeeRepository, IProjectRepository projectRepository)
        {
            this.applicationService = applicationService;
            this.employeeRepository = employeeRepository;
            this.projectRepository=projectRepository;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(dynamic parameters)
        {
            return await employeeRepository.GetEmployeesAsync(parameters);
        }

        public Task<IEnumerable<Employee>> GetEmployeesByProjectAsync(dynamic parameters)
        {
            return employeeRepository.GetEmployeesByProjectAsync(parameters);
        }

        public Task<IEnumerable<Employee>> GetActiveEmployeesByProjectAsync(dynamic parameters)
        {
            return employeeRepository.GetActiveEmployeesByProjectAsync(parameters);
        }

        public async Task<IEnumerable<Employee>> GetProjectAsignedEvaluatorsAsync(dynamic parameters)
        {
            return await employeeRepository.GetProjectAsignedEvaluatorsAsync(parameters);
        }

        public async Task<IEnumerable<string>> GetEmployeePermissionsAsync(string employeeId)
        {
            var appId = await applicationService.GetApplicationId(new { Application = "Quasar" });
            var parameters = new { employeeId, ApplicationId = appId };
            return await employeeRepository.GetEmployeePermissionsAsync(parameters);
        }

        public async Task<bool> UpdateProjectEvaluatorsAsync(dynamic parameters)
        {
            return await employeeRepository.UpdateProjectEvaluators(parameters);
        }

        public async Task<IEnumerable<Project>> GetProjectsByCompany(dynamic parameters)
        {
            return await projectRepository.GetProjectsByCompanyAsync(parameters);
        }

        public async Task<bool> IsEmployeeEvaluatorAsync(dynamic parameters)
        {
            return await employeeRepository.IsEmployeeEvaluatorAsync(parameters);
        }

        public async Task<IEnumerable<Project>> GetEvaluatorAssignedProjectsAsync(dynamic parameters)
        {
            return await employeeRepository.GetEvaluatorAssignedProjectsAsync(parameters);
        }

        public async Task<IEnumerable<Employee>> GetAgentsWithEvaluationsAsync()
        {
            return await employeeRepository.GetAgentsWithEvaluationsAsync();
        }
    }
}
