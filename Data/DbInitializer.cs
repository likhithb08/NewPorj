using LOCPS.Constants;
using LOCPS.Data;
using LOCPS.Enums;
using LOCPS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LOCPS.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context, ILogger logger)
    {
        await context.Database.MigrateAsync();

        if (!await context.Role.AnyAsync())
        {
            context.Role.AddRange(
                new Role { RoleId = RoleConstants.CustomerRoleId, Roles = Roles.Customer, RoleDescription = "Customer role for loan applicants", IsActive = true },
                new Role { RoleId = RoleConstants.AdminRoleId, Roles = Roles.Admin, RoleDescription = "Admin role for system administration", IsActive = true },
                new Role { RoleId = RoleConstants.LoanOfficerRoleId, Roles = Roles.LoanOfficer, RoleDescription = "Loan Officer role for processing loans", IsActive = true },
                new Role { RoleId = RoleConstants.UnderWriterRoleId, Roles = Roles.UnderWriter, RoleDescription = "UnderWriter role for loan underwriting", IsActive = true }
            );
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded default roles.");
        }

        if (!await context.Users.AnyAsync())
        {
            var hasher = new PasswordHasher<User>();
            var admin = new User
            {
                UserName = "admin",
                Email = "admin@locps.in",
                FullName = "System Administrator",
                PhoneNumber = "9000000001",
                RoleId = RoleConstants.AdminRoleId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
            admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");

            var officer = new User
            {
                UserName = "officer",
                Email = "officer@locps.in",
                FullName = "Loan Officer Demo",
                PhoneNumber = "9000000002",
                RoleId = RoleConstants.LoanOfficerRoleId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
            officer.PasswordHash = hasher.HashPassword(officer, "Officer@123");

            var underwriter = new User
            {
                UserName = "underwriter",
                Email = "underwriter@locps.in",
                FullName = "Underwriter Demo",
                PhoneNumber = "9000000003",
                RoleId = RoleConstants.UnderWriterRoleId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
            underwriter.PasswordHash = hasher.HashPassword(underwriter, "Under@123");

            var customer = new User
            {
                UserName = "customer",
                Email = "customer@locps.in",
                FullName = "Rajesh Kumar",
                PhoneNumber = "9000000004",
                RoleId = RoleConstants.CustomerRoleId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
            customer.PasswordHash = hasher.HashPassword(customer, "Customer@123");

            context.Users.AddRange(admin, officer, underwriter, customer);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded default users.");
        }

        if (!await context.LoanProducts.AnyAsync())
        {
            var adminId = await context.Users.Where(u => u.RoleId == RoleConstants.AdminRoleId).Select(u => u.UserId).FirstAsync();
            context.LoanProducts.AddRange(
                new LoanProduct { ProductName = "Home Purchase Loan", ProductDescription = "Residential property financing", MinAmount = 500000, MaxAmount = 50000000, InterestRate = 8.5m, MaxTenureMonths = 240, ProcessingFee = 10000, CreatedByUserId = adminId },
                new LoanProduct { ProductName = "Personal Express Loan", ProductDescription = "Quick personal loans", MinAmount = 50000, MaxAmount = 2000000, InterestRate = 12.5m, MaxTenureMonths = 60, ProcessingFee = 2500, CreatedByUserId = adminId },
                new LoanProduct { ProductName = "Commercial Equipment Loan", ProductDescription = "Business equipment financing", MinAmount = 200000, MaxAmount = 10000000, InterestRate = 10.0m, MaxTenureMonths = 84, ProcessingFee = 5000, CreatedByUserId = adminId }
            );
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded loan products.");
        }
    }
}
