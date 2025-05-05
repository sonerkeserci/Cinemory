using Microsoft.EntityFrameworkCore;

namespace Cinemory.Data
{
    public class CinemoryDbContext:DbContext
    {
        public CinemoryDbContext(DbContextOptions<CinemoryDbContext> options) : base(options) { }

    }
}
