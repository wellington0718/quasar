namespace Application.UseCases.EvaluationsTypeUseCase
{
    public class EvaluationTypesUseCases : IEvaluationTypesUseCases
    {
        private readonly IEvaluationTypesRepository evaluationTypesRepository;

        public EvaluationTypesUseCases(IEvaluationTypesRepository evaluationTypesRepository)
        {
            this.evaluationTypesRepository=evaluationTypesRepository;
        }

        public async Task<IEnumerable<EvaluationComponent>> GetEvaluationTypeComponentsAsync(dynamic parameters)
        {
           return await evaluationTypesRepository.GetEvaluationTypeComponetsAsync(parameters);
        }

        public async Task<IEnumerable<Employee>> GetEvaluationTypeEvaluatorsAsync(dynamic parameters)
        {
            return await evaluationTypesRepository.GetEvaluationTypeEvaluatorsAsync(parameters);
        }

        public async Task<IEnumerable<EvaluationType>> GetEvaluationTypesAsync(dynamic parameters)
        {
            return await evaluationTypesRepository.GetEvaluationTypesAsync(parameters);
        }

        public async Task<bool> SaveEvaluationTypeAsync(dynamic parameters)
        {
            return await evaluationTypesRepository.SaveEvaluationTypeAsync(parameters);
        }

        public async Task<bool> SaveEvaluationTypeComponentAsync(dynamic parameters)
        {
            return await evaluationTypesRepository.SaveEvaluationTypeComponentAsync(parameters);
        }

        public async Task<bool> UpdateEvaluationTypeEvaluators(dynamic parameters)
        {
           return await evaluationTypesRepository.UpdateEvaluationTypeEvaluators(parameters);
        }
    }
}
