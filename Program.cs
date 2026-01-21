using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Avocado_shop.Areas.Identity.Data;
using Avocado_shop.Services;
using Avocado_shop.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AvocadoDBContextConnection")
    ?? throw new InvalidOperationException("Connection string 'AvocadoDBContextConnection' not found.");

builder.Services.AddDbContext<AvocadoDBContext>(options =>
    options.UseSqlite(connectionString));

// Identity - uproszczone (bez potwierdzania emaila)
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AvocadoDBContext>();

// MVC + Razor
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Session (dla koszyka)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Seed roles and admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbInitializer.SeedAsync(services, builder.Configuration);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Seeding error: {ex}");
    }
}

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();