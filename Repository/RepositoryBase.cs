using sigma_backend.Data;

namespace sigma_backend.Repository
{
    public abstract class RepositoryBase
    {
        protected readonly ApplicationDbContext _context;
        public RepositoryBase(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}