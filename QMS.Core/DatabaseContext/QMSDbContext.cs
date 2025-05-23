using Microsoft.EntityFrameworkCore;
using QMS.Core.Models;

namespace QMS.Core.DatabaseContext
{
    public class QMSDbContext : DbContext
    {
        public QMSDbContext(DbContextOptions<QMSDbContext> options) : base(options) { }

        public DbSet<Users> User { get; set; }
        public DbSet<UserRoles> UserRole { get; set; }
        public DbSet<Vendor> Vendor { get; set; }
        public DbSet<VendorResViewModel> Vendor_List { get; set; }
        public DbSet<ProductCodeDetailViewModel> ProductCode { get; set; }
        public DbSet<CertificateMaster> CertificateMaster { get; set; }
        public DbSet<ThirdPartyInspection> ThirdPartyInspections { get; set; }
        public DbSet<InspectionResult> InspectionResults { get; set; }
        public DbSet<CertificationDetail> CertificationDetails { get; set; }
        public DbSet<ThirdPartyCertificateMaster> ThirdPartyCertificateMasters { get; set; }
        public DbSet<ThirdPartyTestReport> ThirdPartyTestReports { get; set; }
        public DbSet<CertificationDetailViewModel> CertificationDetailViewModel { get; set; }
        public DbSet<ThirdPartyTestReportViewModel> ThirdPartyTestReportViewModels { get; set; }
        public DbSet<BisProject_Tracker> BisProject_Tracker { get; set; }
        public DbSet<NPITracker> NPITracker { get; set; }
        public DbSet<VenBISCertificateViewModel> venBISCertificateViewModels { get; set; }
        public DbSet<VenBISCertificate> VenBISCertificates { get; set; }
        public DbSet<CSOTracker> CSOTracker { get; set; }
        public DbSet<PDITracker> PDITracker { get; set; }
        public DbSet<DNTracker> DeviationNote { get; set; }
        public DbSet<ImprovementTracker> ImprovementTracker { get; set; }
        public DbSet<KaizenTracker> KaizenTracker { get; set; }
        public DbSet<SPMReport> SPMReports { get; set; }
        public class InspectionResult
        {
            public int InspectionID { get; set; }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mark InspectionResult as keyless
            modelBuilder.Entity<InspectionResult>().HasNoKey();
            modelBuilder.Entity<CertificationDetailViewModel>().HasNoKey(); // ← Add this
            // Seed roles
            modelBuilder.Entity<UserRoles>().HasData(
                new UserRoles { Id = 1, RoleName = "Admin" },
                new UserRoles { Id = 2, RoleName = "Manager" }
            );

            // Seed default admin user
            modelBuilder.Entity<Users>().HasData(
                new Users
                {
                    Id = 1,
                    Name = "Admin",
                    Email = "sysadmin@gmail.com",
                    Username = "sysadmin@gmail.com",
                    Password = "12345",
                    RoleId = 1
                }
            );
        }
    }
}
