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

// Repository Layer Dependency Injection registration
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILoanApplicationRepository, LoanApplicationRepository>();
builder.Services.AddScoped<IApprovalRepository, ApprovalRepository>();
builder.Services.AddScoped<ICreditEvaluationRepository, CreditEvaluationRepository>();
builder.Services.AddScoped<IDisbursmentRepository, DisbursmentRepository>();
builder.Services.AddScoped<IKycRepository, KycRepository>();
builder.Services.AddScoped<ILoanProductRepository, LoanProductRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
// Bug 3 fix: register the new EMI-specific repository (was using IGenericRepository<Emi> with no filtered query)
builder.Services.AddScoped<IEmiRepository, EmiRepository>();

// Service Layer Dependency Injection registration
builder.Services.AddScoped<IUserServices, UserServices>();
// Bug 2 fix: IUserService (new interface in IServiceInterfaces.cs / CoreServices.cs) was never registered
builder.Services.AddScoped<IUserService, UserService>();
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

// Bug 1 fix: The old inline seeder (lines 66-93) called AddRange but NEVER called SaveChangesAsync
// so roles were never actually saved to the database.
// Also, the existing DbInitializer (which seeds roles, users, and loan products) was never called.
// Now we call it correctly using an async startup scope.
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
