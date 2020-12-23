using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProductDb.Common.Entities;
using ProductDb.Common.Enums;
using ProductDb.Data.BiggBrandsDb.ProductDocks;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ProductDb.Data.BiggBrandsDb
{
    public class BiggBrandsDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BiggBrandsDbContext(DbContextOptions<BiggBrandsDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public DbSet<Attributes> Attributes { get; set; }
        public DbSet<AttributesValue> AttributesValue { get; set; }
        public DbSet<Brand> Brand { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<CategoryAttributeMapping> CategoryAttributeMapping { get; set; }
        public DbSet<Language> Language { get; set; }
        public DbSet<LanguageValues> LanguageValues { get; set; }
        public DbSet<ParentProduct> ParentProduct { get; set; }
        public DbSet<Pictures> Pictures { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductVariant> ProductVariant { get; set; }
        public DbSet<ProductAttributeMapping> ProductAttributeMapping { get; set; }
        public DbSet<ProductAttributeValue> ProductAttributeValue { get; set; }
        public DbSet<Store> Store { get; set; }
        public DbSet<StoreProductMapping> StoreProductMapping { get; set; }
        public DbSet<StoreWarehouseMapping> StoreWarehouseMapping { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<WarehouseProductStock> WarehouseProductStock { get; set; }
        public DbSet<WarehouseQuery> WarehouseQuery { get; set; }
        public DbSet<WarehouseType> WarehouseType { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }
        public DbSet<ProductDock> ProductDock { get; set; }
        public DbSet<ProductDockPictures> ProductDockPictures { get; set; }
        public DbSet<VatRate> VatRate { get; set; }
        public DbSet<ParentProductDock> ParentProductDock { get; set; }
        public DbSet<ProductDockCategory> ProductDockCategory { get; set; }
        public DbSet<ProductGroup> ProductGroup { get; set; }
        public DbSet<ProductBuyingPriceHistory> ProductBuyingPriceHistory { get; set; }        
        public DbSet<Formula> Formula { get; set; }
        public DbSet<FormulaGroup> FormulaGroup { get; set; }
        public DbSet<Margin> Margin { get; set; }
        public DbSet<MarginType> MarginType { get; set; }
        public DbSet<CargoType> CargoType { get; set; }
        public DbSet<Cargo> Cargo { get; set; }
        public DbSet<TransportationType> TransportationType { get; set; }
        public DbSet<Transportation> Transportation { get; set; }

        #region Store Formulas
        
        public DbSet<ProductDockAttribute> ProductDockAttribute { get; set; }
        public DbSet<StoreCategoryMapping> StoreCategoryMapping { get; set; }
        public DbSet<ErpCompany> ErpCompany { get; set; }
        public DbSet<ProductErpCompanyMapping> ProductErpCompanyMapping { get; set; }

        #endregion

        private void PreInsertListener()
        {
            foreach (var entity in ChangeTracker.Entries<EntityBase>().Where(x => x.State == EntityState.Added).ToList())
            {
                entity.Entity.CreatedDate = DateTime.Now;
                entity.Entity.State = (int)State.Active;
                try
                {
                    entity.Entity.ProcessedBy = int.Parse(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
                }
                catch
                {
                    entity.Entity.ProcessedBy = 0;
                }
               
            }
        }

        private void UpdateListener()
        {
            foreach (var entity in ChangeTracker.Entries<EntityBase>().Where(x => x.State == EntityState.Modified).ToList())
            {
                entity.Entity.UpdatedDate = DateTime.Now;
                try
                {
                    entity.Entity.ProcessedBy = int.Parse(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
                }
                catch
                {
                    entity.Entity.ProcessedBy = 0;
                }
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            PreInsertListener();
            UpdateListener();
            return base.SaveChangesAsync();
        }

        public override int SaveChanges()
        {
            PreInsertListener();
            UpdateListener();
            return base.SaveChanges();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => !string.IsNullOrEmpty(type.Namespace))
                .Where(type => type.BaseType != null && type.BaseType.IsGenericType
                && type.BaseType.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>));
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.ApplyConfiguration(configurationInstance);
            }
            base.OnModelCreating(modelBuilder);
        }


    }
}
