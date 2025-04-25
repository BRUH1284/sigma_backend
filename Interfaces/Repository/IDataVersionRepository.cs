using sigma_backend.Enums;
using sigma_backend.Models;

namespace sigma_backend.Interfaces.Repository
{
    public interface IDataVersionRepository
    {
        public Task<DataVersion> CreateOrUpdate(DataResource dataResource);
        public Task<DateTime?> LastUpdatedAt(DataResource dataResource);
    }
}