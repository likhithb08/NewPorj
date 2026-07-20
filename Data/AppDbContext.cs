using LOCPS.Models;
using Microsoft.EntityFrameworkCore;
namespace LOCPS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<LoanProduct> LoanProducts { get; set; }
        public DbSet<LoanApplication> LoanApplications { get; set; }
        public DbSet<Kyc> Kyc { get; set; }
        public DbSet<CreditEvaluation> CreditEvaluation { get; set; }

        public DbSet<Document> Document { get; set; }
        public DbSet<Approval> Approval { get; set; }

        public DbSet<Disbursment> Disbursments { get; set; }

        public DbSet<Emi> Emis { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Auditlog> Auditlogs { get; set; }
        public DbSet<ScoringConfig> ScoringConfigs { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure the relationships and constraints here if needed
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);
            modelBuilder.Entity<Permission>()
                .HasOne(p => p.Role)
                .WithMany(r => r.Permissions)
                .HasForeignKey(p => p.RoleId);
            modelBuilder.Entity<Role>()
                .Property(r => r.RoleId)
                .ValueGeneratedNever();
            modelBuilder.Entity<LoanApplication>()
                .HasOne(la => la.Customer)
                .WithMany()
                .HasForeignKey(la => la.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LoanApplication>()
                .Property(la => la.RequestedAmount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<LoanApplication>()
                .Property(la => la.ApprovedAmount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<LoanApplication>()
                .Property(la => la.AnnualIncome)
                .HasPrecision(18, 2);
            modelBuilder.Entity<LoanApplication>()
                .HasOne(la => la.Product)
                .WithMany()
                .HasForeignKey(la => la.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LoanApplication>()
                .HasOne(la => la.CreatedBy)
                .WithMany()
                .HasForeignKey(la => la.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LoanProduct>()
                .Property(lp => lp.MinAmount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<LoanProduct>()
                .Property(lp => lp.MaxAmount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<LoanProduct>()
                .Property(lp => lp.InterestRate)
                .HasPrecision(18, 2);
            modelBuilder.Entity<LoanProduct>()
                .Property(lp => lp.ProcessingFee)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Kyc>()
                .HasOne(k => k.Application)
                .WithMany()
                .HasForeignKey(k => k.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Kyc>()
                .HasOne(k => k.Verification)
                .WithMany()
                .HasForeignKey(k => k.VerifiedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CreditEvaluation>()
                 .HasOne(k => k.Application)
                 .WithMany()
                 .HasForeignKey(k => k.ApplicationId)
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CreditEvaluation>()
                .HasOne(c => c.EvaluatedBy)
                .WithMany()
                .HasForeignKey(c => c.EvaluatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<CreditEvaluation>()
                .Property(c => c.DebitToIncomeRatio)
                .HasPrecision(5, 2);

            modelBuilder.Entity<CreditEvaluation>()
                .Property(c => c.ExistingLiabilities)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Application)
                .WithMany()
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Kyc)
                .WithMany()
                .HasForeignKey(d => d.KycId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Customer)
                .WithMany()
                .HasForeignKey(d => d.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Verifier)
                .WithMany()
                .HasForeignKey(d => d.VerifiedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Approval>()
                .HasOne(d => d.Application)
                .WithMany()
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Approval>()
                .HasOne(d => d.Approver)
                .WithMany()
                .HasForeignKey(d => d.ApprovedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Approval>()
                .Property(p => p.ApprovedAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Approval>()
                .Property(p => p.ApprovedInterestRate)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Disbursment>()
                .HasOne(d => d.LoanApplication)
                .WithMany()
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Disbursment>()
               .HasOne(d => d.User)
               .WithMany()
               .HasForeignKey(d => d.ProcessedByUserID)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Disbursment>()
                .Property(d => d.AmountApproved)
                .HasPrecision(13, 2);

            modelBuilder.Entity<Emi>()
                .Property(e => e.EmiAmount)
                .HasPrecision(13,2);

            modelBuilder.Entity<Emi>()
                .Property(e => e.PenaltyAmount)
                .HasPrecision(13,2);

            modelBuilder.Entity<Emi>()
                .Property(e => e.PaidAmount)
                .HasPrecision(13,2);

            modelBuilder.Entity<Emi>()
                .HasOne(e => e.Application)
                .WithMany()
                .HasForeignKey(e => e.ApplicationID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.RelatedApplication)
                .WithMany()
                .HasForeignKey(n => n.RelatedApplicationId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Auditlog>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ScoringConfig>()
                .HasOne(s => s.UpdatedByUser)
                .WithMany()
                .HasForeignKey(s => s.UpdatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Ensure ConfigId is never auto-generated (singleton row)
            modelBuilder.Entity<ScoringConfig>()
                .Property(s => s.ConfigId)
                .ValueGeneratedNever();

        }
    }
}
