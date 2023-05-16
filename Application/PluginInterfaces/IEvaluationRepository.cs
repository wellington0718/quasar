namespace Application.PluginInterfaces
{
    public interface IEvaluationRepository
    {
        Task<IEnumerable<EvaluationDTO>> SearchEvaluationsAsync(dynamic parameters);
        Task<IEnumerable<EvaluationDTO>> GetCurrentDayEvaluationsAsync(dynamic parameters);
        Task<IEnumerable<EvaluationDTO>> GetEvaluationChangesHistoryAsync(dynamic parameters);
        Task<IEnumerable<QualityRateDTO>> GetAgentsEvaluationsReportAsync(dynamic parameters);
        Task<IEnumerable<QualityRateDTO>> GetEvaluationsTypesReportAsync(dynamic parameters);
        Task<IEnumerable<QualityRateDTO>> GetGeneralReportAsync(dynamic parameters);
        Task<IEnumerable<EvaluationScoreDetail>> GetEvaluationScoreDetailAsync(dynamic parameters);
        Task<(float, int)> GetEvaluationsAVGByMonthAsync(dynamic parameters);
        Task<bool> SaveEvaluationAsync(dynamic parameters);
        Task<bool> SaveEvaluationScoreDetailAsync(dynamic parameters);
        Task<IEnumerable<EvaluationScoreDetail>> GetEvaluationTypesComponetsErrorsAsync(dynamic parameters);
    }
}
