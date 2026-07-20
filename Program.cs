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

// Add MVC services (no API controllers)
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

// ── Repository Layer ──────────────────────────────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILoanApplicationRepository, LoanApplicationRepository>();
builder.Services.AddScoped<IApprovalRepository, ApprovalRepository>();
builder.Services.AddScoped<ICreditEvaluationRepository, CreditEvaluationRepository>();
builder.Services.AddScoped<IDisbursmentRepository, DisbursmentRepository>();
builder.Services.AddScoped<IKycRepository, KycRepository>();
builder.Services.AddScoped<ILoanProductRepository, LoanProductRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IEmiRepository, EmiRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

// ── Service Layer ─────────────────────────────────────────────────────────────
// NOTE: AuditLogService and NotificationService are registered first because
//       other services depend on them (constructor injection order matters in
//       case of circular dependency resolution — Scoped lifetime avoids issues).
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoanProductService, LoanProductService>();
builder.Services.AddScoped<ILoanApplicationService, LoanApplicationService>();
builder.Services.AddScoped<IKycService, KycService>();
builder.Services.AddScoped<ICreditEvaluationService, CreditEvaluationService>();
builder.Services.AddScoped<IApprovalService, ApprovalService>();
builder.Services.AddScoped<IDisbursementService, DisbursementService>();
builder.Services.AddScoped<IEmiService, EmiService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

var app = builder.Build();

// Seed the database on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await DbInitializer.InitializeAsync(context, logger);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
