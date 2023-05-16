using Domain.Enums;
using SharedKernel.Interfaces;
using System.Data;
using System.Threading.Tasks;

namespace SharedKernel.Services;
public class ValidateCredentialService
{
    private readonly IBaseDataAccess baseDataAccess;

    public ValidateCredentialService(IBaseDataAccess baseDataAccess)
    {
        this.baseDataAccess=baseDataAccess;
    }
    public async Task<bool> Validate(dynamic parameters)
    {
        return await baseDataAccess.QuerySingleAsync<bool, dynamic>(nameof(StoreProcedures.ValidateEmployeeCredential), parameters, CommandType.StoredProcedure);
    }
}

