using HostelManagementSystem.Data;
using HostelManagementSystem.Identity;
using HostelManagementSystem.Services;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using HostelManagementSystem.Settings;
using HostelManagementSystem.Implementation;
using HostelManagementSystem.Repositories;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IHostelRoomService, HostelRoomService>();
builder.Services.AddHttpClient<IPaystackService, PaystackService>();
builder.Services.Configure<PaystackSettings>(
builder.Configuration.GetSection("Paystack"));
builder.Services.Configure<HostelManagementSystem.Settings.EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IHostelApplicationRepository,HostelApplicationRepository>();
builder.Services.AddScoped<IHostelApplicationService,HostelApplicationService>();
builder.Services.AddScoped<IPaymentRepository,PaymentRepository>();
builder.Services.AddScoped<IPaymentService,PaymentService>();
builder.Services.AddScoped<IReportsRepository,ReportsRepository>();
builder.Services.AddScoped<IReportsService,ReportsService>();
builder.Services.AddScoped<IRoomAllocationRepository,RoomAllocationRepository>();
builder.Services.AddScoped<IRoomAllocationService,RoomAllocationService>();
builder.Services.AddScoped<IStudentRepository,StudentRepository>();
builder.Services.AddScoped<IStudentService,StudentService>();
builder.Services.AddScoped<IStudentDashboardRepository,StudentDashboardRepository>();
builder.Services.AddScoped<IStudentDashboardService,StudentDashboardService>();
builder.Services.AddScoped<IContactMessageRepository,ContactMessageRepository>();
builder.Services.AddScoped<IContactMessageService,ContactMessageService>();
builder.Services.AddScoped<IAdminProfileRepository,AdminProfileRepository>();
builder.Services.AddScoped<IAdminProfileService,AdminProfileService>();
builder.Services.AddScoped<IAdminDashboardRepository,AdminDashboardRepository>();
builder.Services.AddScoped<IAdminDashboardService,AdminDashboardService>();
builder.Services.AddScoped<IHostelRoomRepository, HostelRoomRepository>();
builder.Services.AddScoped<IHostelRoomService, HostelRoomService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICustomEmailSender,SmtpEmailSender>();
var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();

    await SeedAdmin.Initialize(services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();