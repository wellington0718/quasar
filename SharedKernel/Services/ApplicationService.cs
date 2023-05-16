using Domain.Enums;
using SharedKernel.Interfaces;
using System.Data;
using System.Threading.Tasks;

namespace SharedKernel.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IBaseDataAccess baseDataAccess;

        public ApplicationService(IBaseDataAccess baseDataAccess)
        {
            this.baseDataAccess = baseDataAccess;
        }

        public async Task<int> GetApplicationId(dynamic parameters)
        {
            return await baseDataAccess.QuerySingleAsync<int, dynamic>(nameof(StoreProcedures.GetApplicationId), parameters, CommandType.StoredProcedure);
        }
    }
}
