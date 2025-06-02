using QMS.Core.DatabaseContext;
using QMS.Core.Repositories.UserRolesRepository;
using QMS.Core.Repositories.UsersRepository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QMS.Core.Services.SystemLogs;
using QMS.Core.Repositories.DashBordRepository;
using QMS.Core.Repositories.DashBoardRepository;
using QMS.Core.Repositories.VendorRepository;
using QMS.Core.Repositories.CertificateMasterRepository;
using QMS.Core.Repositories.ThirdPartyInspectionRepository;
using QMS.Core.Repositories.ThirdPartyCertRepository;
using QMS.Core.Repositories.BisProjectTracRepository;
using QMS.Core.Repositories.NPITrackerRepository;
using QMS.Core.Repositories.CSOTrackerRepository;
using QMS.Core.Repositories.PDITrackerRepository;
using QMS.Core.Repositories.DNTrackerRepository;
using QMS.Core.Repositories.ImprTrackerRepository;
using QMS.Core.Repositories.KaizenTrackerRepository;
using QMS.Core.Repositories.SPMReportRepository;
using QMS.Core.Repositories.RMTCDetailsRepository;
using QMS.Core.Repositories.COPQComplaintDumpRepository;

var builder = WebApplication.CreateBuilder(args);// Configure database connection.
var connstring = builder.Configuration.GetConnectionString("DbConnectionString");
builder.Services.AddDbContext<QMSDbContext>(Options => Options.UseSqlServer(connstring));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// add custom services
builder.Services.AddTransient<IUsersRepository, UsersRepository>();
builder.Services.AddTransient<IUserRolesRepository, UserRolesRepository>();
builder.Services.AddTransient<ISystemLogService, SystemLogService>();
builder.Services.AddTransient<IDashBoardRepository, DashBoardRepository>();
builder.Services.AddTransient<IVendorRepository, VendorRepository>();
builder.Services.AddTransient<ICertificateMasterRepository, CertificateMasterRepository>();
builder.Services.AddTransient<IThirdPartyInspectionRepository, ThirdPartyInspectionRepository>();
builder.Services.AddTransient<IThirdPartyCertRepository, ThirdPartyCertRepository>();
builder.Services.AddTransient<IBisProjectTracRepository, BisProjectTracRepository>();
builder.Services.AddTransient<INPITrackerRepository, NPITrackerRepository>();
builder.Services.AddTransient<IPDITrackerRepository, PDITrackerRepository>();
builder.Services.AddTransient<ICSOTrackerRepository, CSOTrackerRepository>();
builder.Services.AddTransient<IDNTrackerRepository, DNTrackerRepository>();
builder.Services.AddTransient<IImprTrackerRepository, ImprTrackerRepository>();
builder.Services.AddTransient<IKaizenTrackerRepository, KaizenTrackerRepository>();
builder.Services.AddTransient<ISPMReportRepository, SPMReportRepository>();
builder.Services.AddTransient<IRMTCDetailsRepository, RMTCDetailsRepository>();
builder.Services.AddTransient<IComplaintIndentDumpRepository, ComplaintIndentDumpRepository>();

//

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Unspecified;
    options.LoginPath = "/Account/Login";
    options.SlidingExpiration = true;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(
    options =>
    {
        options.LoginPath = new PathString("/Account/Login");
    });
builder.Services.AddAuthorization();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", builder => builder
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());
});
builder.Services.AddDistributedMemoryCache();
var app = builder.Build();

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
app.UseCors("CorsPolicy");
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();


