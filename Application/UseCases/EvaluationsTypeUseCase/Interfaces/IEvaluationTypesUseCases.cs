namespace Application.UseCases.EvaluationsTypeUseCase.Interfaces
{
    public interface IEvaluationTypesUseCases
    {
        Task<IEnumerable<EvaluationType>> GetEvaluationTypesAsync(dynamic parameters);
        Task<IEnumerable<EvaluationComponent>> GetEvaluationTypeComponentsAsync(dynamic parameters);
        Task<IEnumerable<Employee>> GetEvaluationTypeEvaluatorsAsync(dynamic parameters);
        Task<bool> UpdateEvaluationTypeEvaluators(dynamic parameters);
        Task<bool> SaveEvaluationTypeComponentAsync(dynamic parameters);
        Task<bool> SaveEvaluationTypeAsync(dynamic parameters);
    }
}
