using LOCPS.Constants;
using LOCPS.Data;
using LOCPS.Models;
using LOCPS.Repositories.Implementation;
using LOCPS.Repositories.Interfaces;
using LOCPS.Services.Implementations;
using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = AuthConstants.AuthCookieName;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromHours(AuthConstants.SessionHours);
        options.SlidingExpiration = true;
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.LogoutPath = "/Account/Logout";
    });

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<LOCPS.LocpsRoleAuthorizeFilter>();
})
.AddRazorOptions(options =>
{
    options.ViewLocationExpanders.Add(new LOCPS.RoleBasedViewLocationExpander());
});

builder.Services.AddDbContext<LOCPS.Data.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConn")));

//Repository Layer Dependecy Injection registration
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILoanApplicationRepository, LoanApplicationRepository>();
builder.Services.AddScoped<IApprovalRepository, ApprovalRepository>();
builder.Services.AddScoped<ICreditEvaluationRepository, CreditEvaluationRepository>();
builder.Services.AddScoped<IDisbursmentRepository, DisbursmentRepository>();
builder.Services.AddScoped<IKycRepository, KycRepository>();
builder.Services.AddScoped<ILoanProductRepository, LoanProductRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

//Service Layer Dependecy Injection registration
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<ILoanProductService, LoanProductService>();
builder.Services.AddScoped<ILoanApplicationService, LoanApplicationService>();
builder.Services.AddScoped<IKycService, KycService>();
builder.Services.AddScoped<ICreditEvaluationService, CreditEvaluationService>();
builder.Services.AddScoped<IApprovalService, ApprovalService>();
builder.Services.AddScoped<IDisbursementService, DisbursementService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IEmiService, EmiService>();


var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!context.Role.Any())
    {
        context.Role.AddRange(
                new Role
                {
                    RoleId = 1,
                    Roles = LOCPS.Enums.Roles.Customer
                },
                new Role
                {
                    RoleId = 2,
                    Roles = LOCPS.Enums.Roles.LoanOfficer
                },new Role
                {
                    RoleId = 3,
                    Roles = LOCPS.Enums.Roles.UnderWriter
                },
                new Role
                {
                    RoleId = 4,
                    Roles = LOCPS.Enums.Roles.Admin
                }
            );
    }
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
