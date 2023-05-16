namespace Application.UseCases.EvaluationUseCases
{
    public class EvaluationUseCases : IEvaluationUseCases
    {
        private readonly IEvaluationRepository evaluationRepository;

        public EvaluationUseCases(IEvaluationRepository evaluationRepository)
        {
            this.evaluationRepository = evaluationRepository;
        }

        public async Task<IEnumerable<QualityRateDTO>> GetAgentsEvaluationsReportAsync(dynamic parameters)
        {
            var testList = await evaluationRepository.GetAgentsEvaluationsReportAsync(parameters);

            return testList;
        }

        public async Task<IEnumerable<EvaluationDTO>> GetCurrentDayEvaluationsAsync(dynamic parameters)
        {
            return await evaluationRepository.GetCurrentDayEvaluationsAsync(parameters);
        }

        public Task<IEnumerable<EvaluationDTO>> GetEvaluationChangesHistoryAsync(dynamic parameters)
        {
            return evaluationRepository.GetEvaluationChangesHistoryAsync(parameters);
        }

        public async Task<(float, int)> GetEvaluationsAVGByMonthAsync(dynamic parameters)
        {
            return await evaluationRepository.GetEvaluationsAVGByMonthAsync(parameters);
        }

        public async Task<IEnumerable<EvaluationScoreDetail>> GetEvaluationScoreDetailAsync(dynamic parameters)
        {
            return await evaluationRepository.GetEvaluationScoreDetailAsync(parameters);
        }

        public async Task<IEnumerable<QualityRateDTO>> GetEvaluationsTypesReportAsync(dynamic parameters)
        {
            return await evaluationRepository.GetEvaluationsTypesReportAsync(parameters);
        }

        public async Task<IEnumerable<EvaluationScoreDetail>> GetEvaluationTypesComponetsErrorsAsync(dynamic parameters)
        {
            return await evaluationRepository.GetEvaluationTypesComponetsErrorsAsync(parameters);
        }

        public async Task<IEnumerable<QualityRateDTO>> GetGeneralReport(dynamic parameters)
        {
            return await evaluationRepository.GetGeneralReportAsync(parameters);
        }

        public async Task<bool> SaveEvaluationAsync(dynamic parameters)
        {
            return await evaluationRepository.SaveEvaluationAsync(parameters);
        }

        public async Task<bool> SaveEvaluationScoreDetailAsync(dynamic parameters)
        {
            return await evaluationRepository.SaveEvaluationScoreDetailAsync(parameters);
        }

        public async Task<IEnumerable<EvaluationDTO>> SearchEvaluationsAsync(dynamic parameters)
        {
            return await evaluationRepository.SearchEvaluationsAsync(parameters);
        }
    }
}

