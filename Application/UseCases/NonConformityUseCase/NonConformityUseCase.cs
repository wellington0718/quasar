using Application.UseCases.NonConformityUseCase.Interfaces;

namespace Application.UseCases.NonConformityUseCase;

public class NonConformityUseCase : INonConformityUseCase
{
    private readonly INonConformityRepository _nonConformityRepository;

    public NonConformityUseCase(INonConformityRepository nonConformityRepository)
    {
        _nonConformityRepository = nonConformityRepository;
    }

    public async Task<bool> SaveNonConformityActionAsync(dynamic parameters)
    {
        var res = await _nonConformityRepository.SaveNonConformityActionAsync(parameters);
        return res;
    }

    public async Task<bool> SaveNonConformityAsync(dynamic parameters)
    {
        var res = await _nonConformityRepository.SaveNonConformityAsync(parameters);
        return res;
    }

    public async Task<IEnumerable<NonConformity>> SearchNonConformitiesAsync(dynamic parameters)
    {
        return await _nonConformityRepository.SearchNonConformitiesAsync(parameters);
    }

    public async Task<IEnumerable<NonConformityAction>> SearchNonConformityActionsAsync(dynamic parameters)
    {
        return await _nonConformityRepository.SearchNonConformityActionsAsync(parameters);
    }
}
