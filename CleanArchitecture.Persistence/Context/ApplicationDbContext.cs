using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Entities.Auth;
using CleanArchitecture.Domain.Entities.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly string _currentUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, string currentUserId) : base(options)
        {
            _currentUserId = currentUserId;
        }

        public DbSet<Status> TblStatus { get; set; }
        public DbSet<ExpenseType> TblExpenseType { get; set; }
        public DbSet<ExpenseCategory> TblExpenseCategories { get; set; }
        public DbSet<UserExpense> TblUserExpenses { get; set; }
        public DbSet<PurchaseHistory> TblPlanPurchaseHistory { get; set; }
        public DbSet<Address> TblAddress { get; set; }
        public DbSet<AddressExpenses> TblAddressExpenses { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.UtcNow;
                        entry.Entity.CreatedBy = _httpContextAccessor.HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "System";
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = _httpContextAccessor.HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "System";
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Status Entity
            modelBuilder.Entity<Status>()
                .HasKey(s => s.StatusId);

            modelBuilder.Entity<Status>()
                .Property(s => s.StatusId)
                .HasConversion<int>();


        }
    }
}
