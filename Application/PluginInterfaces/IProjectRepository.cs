namespace Application.PluginInterfaces
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetProjectsByCompanyAsync(dynamic parameters);
    }
}
