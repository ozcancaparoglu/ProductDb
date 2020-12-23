using Microsoft.EntityFrameworkCore;
using PMS.Data.Entities;
using PMS.Data.Entities.Invoice;
using PMS.Data.Entities.Item;
using PMS.Data.Entities.Logs;
using PMS.Data.Entities.Logo;
using PMS.Data.Entities.Order;
using PMS.Data.Entities.Project;

namespace PMS.Data
{
    public class BiggOfficeContext: DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<ItemVatRate> ItemVatRates { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ExOrderItem> ExOrderItems { get; set; }
        public DbSet<ExOrder> ExOrder { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<LogoCompany> LogoCompanies { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Logs> Logs { get; set; }
        public DbSet<OrderTrackingNumber> OrderTrackingNumbers { get; set; }
        public BiggOfficeContext(DbContextOptions<BiggOfficeContext> options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasIndex(a => a.ProjectCode).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
