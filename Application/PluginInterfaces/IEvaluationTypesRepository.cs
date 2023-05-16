namespace Application.PluginInterfaces
{
    public interface IEvaluationTypesRepository
    {
        Task<bool> UpdateEvaluationTypeEvaluators(dynamic parameters);
        Task<IEnumerable<Employee>> GetEvaluationTypeEvaluatorsAsync(dynamic parameters);
        Task<bool> SaveEvaluationTypeComponentAsync(dynamic parameters);
        Task<bool> SaveEvaluationTypeAsync(dynamic parameters);
        Task<IEnumerable<EvaluationType>> GetEvaluationTypesAsync(dynamic parameters);
        Task<IEnumerable<Employee>> GetEvaluationTypesAgentsAsync(dynamic parameters);
        Task<IEnumerable<EvaluationComponent>> GetEvaluationTypeComponetsAsync(dynamic parameters);
    }
}
