using Microsoft.EntityFrameworkCore;

namespace ProductDb.Data.NopDb
{
    public class NopDbContext : DbContext
    {
        public NopDbContext(DbContextOptions<NopDbContext> options) : base(options)
        {
        }

        public DbSet<ProductNop> ProductNop { get; set; }
        public DbSet<ProductPictureMappingNop> ProductPictureMappingNop { get; set; }
        public DbSet<PictureNop> PictureNop { get; set; }
        public DbSet<LocalizedPropertyNop> LocalizedPropertyNop { get; set; }

    }
}
