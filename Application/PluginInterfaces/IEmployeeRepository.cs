namespace Application.PluginInterfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetEmployeesAsync(dynamic parameters);
        Task<IEnumerable<Project>> GetEvaluatorAssignedProjectsAsync(dynamic parameters);
        Task<bool> UpdateProjectEvaluators(dynamic parameters);
        Task<IEnumerable<Employee>> GetEmployeesByProjectAsync(dynamic parameters);
        Task<IEnumerable<Employee>> GetActiveEmployeesByProjectAsync(dynamic parameters);
        Task<IEnumerable<Employee>> GetProjectAsignedEvaluatorsAsync(dynamic parameters);
        Task<IEnumerable<Employee>> GetAgentsWithEvaluationsAsync();
        Task<IEnumerable<string>> GetEmployeePermissionsAsync(dynamic parameters);
        Task<bool> IsEmployeeEvaluatorAsync(dynamic parameters);
    }
}
