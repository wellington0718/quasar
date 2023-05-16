namespace Infrastructure.Repositories
{
    public class EvaluationTypesRepository : IEvaluationTypesRepository
    {
        private readonly IBaseDataAccess baseDataAccess;

        public EvaluationTypesRepository(IBaseDataAccess baseDataAccess)
        {
            this.baseDataAccess = baseDataAccess;
        }

        public async Task<IEnumerable<EvaluationComponent>> GetEvaluationTypeComponetsAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<EvaluationComponent, dynamic>(nameof(StoreProcedures.GetEvaluationTypeComponets), parameters, CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<EvaluationType>> GetEvaluationTypesAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<EvaluationType, dynamic>(nameof(StoreProcedures.GetEvaluationTypes), parameters, CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Employee>> GetEvaluationTypeEvaluatorsAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<Employee, dynamic>(nameof(StoreProcedures.GetEvaluationTypeEvaluators), parameters, CommandType.StoredProcedure);
        }

        public async Task<bool> SaveEvaluationTypeComponentAsync(dynamic parameters)
        {
            var rowsAffected = await baseDataAccess.SaveDataAsync<dynamic>(nameof(StoreProcedures.SaveEvaluationTypeComponent), parameters, CommandType.StoredProcedure);
            return rowsAffected != 0;
        }
        public async Task<bool> SaveEvaluationTypeAsync(dynamic parameters)
        {
            var rowsAffected = await baseDataAccess.SaveDataAsync<dynamic>(nameof(StoreProcedures.SaveEvaluationType), parameters, CommandType.StoredProcedure);
            return rowsAffected != 0;
        }

        public async Task<IEnumerable<Employee>> GetEvaluationTypesAgentsAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<Employee, dynamic>(nameof(StoreProcedures.GetEvaluationTypesAgents), parameters, CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateEvaluationTypeEvaluators(dynamic parameters)
        {
            var result = await baseDataAccess.SaveDataAsync<dynamic>(nameof(StoreProcedures.UpdateEvaluationTypeEvaluators), parameters, CommandType.StoredProcedure);
            return result != 0;
        }
    }
}
