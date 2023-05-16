namespace Application.UseCases.NonConformityUseCase.Interfaces;

public interface INonConformityUseCase
{
    Task<bool> SaveNonConformityAsync(dynamic parameters);
    Task<bool> SaveNonConformityActionAsync(dynamic parameters);
    Task<IEnumerable<NonConformity>> SearchNonConformitiesAsync(dynamic parameters);
    Task<IEnumerable<NonConformityAction>> SearchNonConformityActionsAsync(dynamic parameters);
}
