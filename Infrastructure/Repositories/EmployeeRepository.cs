namespace Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IBaseDataAccess baseDataAccess;

        public EmployeeRepository(IBaseDataAccess baseDataAccess)
        {
            this.baseDataAccess = baseDataAccess;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<Employee, dynamic>(nameof(StoreProcedures.GetEmployees), parameters, CommandType.StoredProcedure);
        }  
        
        public async Task<IEnumerable<Employee>> GetAgentsWithEvaluationsAsync()
        {
            return await baseDataAccess.LoadDataAsync<Employee, dynamic>(nameof(StoreProcedures.GetAgentsWithEvaluations), new { }, CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByProjectAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<Employee, dynamic>(nameof(StoreProcedures.GetEmployeesByProject), parameters, CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Employee>> GetActiveEmployeesByProjectAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<Employee, dynamic>(nameof(StoreProcedures.GetActiveEmployeesByProject), parameters, CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Employee>> GetProjectAsignedEvaluatorsAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<Employee, dynamic>(nameof(StoreProcedures.GetProjectAsignedEvaluators), parameters, CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<string>> GetEmployeePermissionsAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<string, dynamic>(nameof(StoreProcedures.GetEmployeePermissions), parameters, CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateProjectEvaluators(dynamic parameters)
        {
            var result = await baseDataAccess.SaveDataAsync<dynamic>(nameof(StoreProcedures.UpdateProjectEvaluators), parameters, CommandType.StoredProcedure);
            return result != 0;
        }

        public async Task<bool> IsEmployeeEvaluatorAsync(dynamic parameters)
        {
            return await baseDataAccess.QuerySingleAsync<bool, dynamic>(nameof(StoreProcedures.IsEmployeeEvaluator), parameters, CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Project>> GetEvaluatorAssignedProjectsAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<Project, dynamic>(nameof(StoreProcedures.GetEvaluatorAssignedProjects), parameters, CommandType.StoredProcedure);
        }
    }
}
