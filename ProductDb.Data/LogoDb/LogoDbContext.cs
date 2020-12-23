using Microsoft.EntityFrameworkCore;

namespace ProductDb.Data.LogoDb
{
    public class LogoDbContext : DbContext
    {

        public LogoDbContext(DbContextOptions<LogoDbContext> options) : base(options)
        {
        }

        public DbSet<URUN_WEB_STOK> URUN_WEB_STOK { get; set; }
        public DbSet<ProductStockModel> ProductStockModel { get; set; }

    }
}
