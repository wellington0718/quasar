namespace Infrastructure.Repositories
{
    public class EvaluationRepository : IEvaluationRepository
    {
        private readonly IBaseDataAccess baseDataAccess;

        public EvaluationRepository(IBaseDataAccess baseDataAccess)
        {
            this.baseDataAccess = baseDataAccess;
        }

        public async Task<IEnumerable<EvaluationDTO>> SearchEvaluationsAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<EvaluationDTO, dynamic>(nameof(StoreProcedures.SearchEvaluations), parameters, CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<EvaluationDTO>> GetEvaluationChangesHistoryAsync(dynamic parameters)
        {

            return await baseDataAccess.LoadDataAsync<EvaluationDTO, dynamic>(nameof(StoreProcedures.GetEvaluationChangesHistory), parameters, CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<EvaluationDTO>> GetCurrentDayEvaluationsAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<EvaluationDTO, dynamic>(nameof(StoreProcedures.GetCurrentDayEvaluations), parameters, CommandType.StoredProcedure);
        }

        public async Task<(float, int)> GetEvaluationsAVGByMonthAsync(dynamic parameters)
        {
            return await baseDataAccess.QuerySingleAsync<(float, int), dynamic>(nameof(StoreProcedures.GetEvaluationsAVGByMonth), parameters, CommandType.StoredProcedure);
        }

        public async Task<bool> SaveEvaluationAsync(dynamic parameters)
        {
            var rowsAffected = await baseDataAccess.SaveDataAsync<dynamic>(nameof(StoreProcedures.SaveEvaluation), parameters, CommandType.StoredProcedure);
            return rowsAffected != 0;
        }

        public async Task<bool> SaveEvaluationScoreDetailAsync(dynamic parameters)
        {
            var rowsAffected = await baseDataAccess.SaveDataAsync<dynamic>(nameof(StoreProcedures.SaveEvaluationScoreDetail), parameters, CommandType.StoredProcedure);
            return rowsAffected != 0;
        }

        public async Task<IEnumerable<EvaluationScoreDetail>> GetEvaluationScoreDetailAsync(dynamic parameters)
        {
           return await baseDataAccess.LoadDataAsync<EvaluationScoreDetail, dynamic>(nameof(StoreProcedures.GetEvaluationScoreDetail), parameters, CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<QualityRateDTO>> GetAgentsEvaluationsReportAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<QualityRateDTO, dynamic>(nameof(StoreProcedures.GetAgentsEvaluationsReport), parameters, CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<QualityRateDTO>> GetEvaluationsTypesReportAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<QualityRateDTO, dynamic>(nameof(StoreProcedures.GetEvaluationsTypesReport), parameters, CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<QualityRateDTO>> GetGeneralReportAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<QualityRateDTO, dynamic>(nameof(StoreProcedures.GetGeneralReport), parameters, CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<EvaluationScoreDetail>> GetEvaluationTypesComponetsErrorsAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<EvaluationScoreDetail, dynamic>(nameof(StoreProcedures.GetEvaluationTypesComponetsErrors), parameters, CommandType.StoredProcedure);
        }
    }
}
