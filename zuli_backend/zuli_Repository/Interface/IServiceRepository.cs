using System;
using System.Threading.Tasks;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IServiceRepository
    {
        Task<int> CreateService(ServiceEntity service);
        Task<ServiceEntity?> GetServiceByFlightId(Guid flightId);
    }
}
