namespace Infrastructure.Repositories
{
    public class NonConformityRepository : INonConformityRepository
    {
        private readonly IBaseDataAccess baseDataAccess;

        public NonConformityRepository(IBaseDataAccess baseDataAccess)
        {
            this.baseDataAccess = baseDataAccess;
        }

        public async Task<IEnumerable<Project>> GetProjectsByCompanyAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<Project, dynamic>(nameof(StoreProcedures.GetProjectsByCompany), parameters, CommandType.StoredProcedure);
        }

        public async Task<bool> SaveNonConformityActionAsync(dynamic parameters)
        {
            var rowsAffected = await baseDataAccess.SaveDataAsync(nameof(StoreProcedures.SaveNonConformityAction), parameters, CommandType.StoredProcedure);
            return rowsAffected != 0;
        }

        public async Task<bool> SaveNonConformityAsync(dynamic parameters)
        {
            var rowsAffected = await baseDataAccess.SaveDataAsync(nameof(StoreProcedures.SaveNonConformity), parameters, CommandType.StoredProcedure);
            return rowsAffected != 0;
        }

        public async Task<IEnumerable<NonConformity>> SearchNonConformitiesAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<NonConformity, dynamic>(nameof(StoreProcedures.SearchNonConformities), parameters, CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<NonConformityAction>> SearchNonConformityActionsAsync(dynamic parameters)
        {
            return await baseDataAccess.LoadDataAsync<NonConformityAction, dynamic>(nameof(StoreProcedures.SearchNonConformityActions), parameters, CommandType.StoredProcedure);
        }
    }
}
