using Domain.Models;
using System.Threading.Tasks;

namespace SharedKernel.Interfaces
{
    public interface IApplicationService
    {
        Task<int> GetApplicationId(dynamic parameters);
    }
}
