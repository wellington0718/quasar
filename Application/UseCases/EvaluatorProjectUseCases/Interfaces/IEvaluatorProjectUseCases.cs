namespace Application.UseCases.EvaluatorProjectUseCases.Interfaces
{
    public interface IEvaluatorProjectUseCases
    {
        Task<bool> UpdateProjectEvaluatorsAsync(dynamic parameters);
        Task<IEnumerable<Project>> GetProjectsByCompany(dynamic parameters);
        Task<IEnumerable<Project>> GetEvaluatorAssignedProjectsAsync(dynamic parameters);
        Task<IEnumerable<Employee>> GetProjectAsignedEvaluatorsAsync(dynamic parameters);
        Task<bool> IsEmployeeEvaluatorAsync(dynamic parameters);
        Task<IEnumerable<Employee>> GetActiveEmployeesByProjectAsync(dynamic parameters);
        Task<IEnumerable<Employee>> GetEmployeesByProjectAsync(dynamic parameters);
        Task<IEnumerable<Employee>> GetEmployeesAsync(dynamic parameters);
        Task<IEnumerable<Employee>> GetAgentsWithEvaluationsAsync();
        Task<IEnumerable<string>> GetEmployeePermissionsAsync(string employeeId);
    }
}
