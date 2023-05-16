namespace Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly IBaseDataAccess baseDataAccess;

        public ProjectRepository(IBaseDataAccess baseDataAccess)
        {
            this.baseDataAccess = baseDataAccess;
        }

        public async Task<IEnumerable<Project>> GetProjectsByCompanyAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<Project, dynamic>(nameof(StoreProcedures.GetProjectsByCompany), parameters, CommandType.StoredProcedure);
        }
    }
}
