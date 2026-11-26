using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MembersOnly.Web.Data;
using MembersOnly.Web.Models;
using MembersOnly.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Store Identity data in SQLite for learning.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Identity")));

// Match the password rules from the original app.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = false;

    // Use email as the unique login identifier.
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Cookie for signed-in users (Step 5 expands this).
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Business Central client, registered per request.
builder.Services.AddScoped<IBusinessCentralClient, BusinessCentralClient>();

// builder.Services.AddHttpClient("BusinessCentral");
builder.Services.AddHttpClient("BusinessCentral")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseDefaultCredentials = true
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Order matters: authentication must run before authorization.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();