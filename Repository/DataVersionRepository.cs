using sigma_backend.Data;
using sigma_backend.Enums;
using sigma_backend.Interfaces.Repository;
using sigma_backend.Models;

namespace sigma_backend.Repository
{
    public class DataVersionRepository : RepositoryBase, IDataVersionRepository
    {
        public DataVersionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<DataVersion> CreateOrUpdate(DataResource dataResource)
        {
            // Get existing data version
            var dataVersion = await _context.DataVersions.FindAsync(dataResource);

            // Create new data version or update existing
            if (dataVersion == null)
            {
                dataVersion = new DataVersion
                {
                    DataResource = dataResource
                };

                await _context.DataVersions.AddAsync(dataVersion);
            }
            else
            {
                dataVersion.LastUpdatedAt = DateTime.UtcNow;
            }

            // Save changes
            await _context.SaveChangesAsync();
            return dataVersion;
        }

        public async Task<DateTime?> LastUpdatedAt(DataResource dataResource)
        {
            return (await _context.DataVersions.FindAsync(dataResource))?.LastUpdatedAt;
        }
    }
}