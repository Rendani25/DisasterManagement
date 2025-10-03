using DisasterManagement.data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity with Roles
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    // Redirect to our custom handler after login
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Configure cookie settings to redirect to our custom handler
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.LogoutPath = "/Identity/Account/Logout";
    options.SlidingExpiration = true;

    // This is key - redirect to our custom handler after login
    options.Events.OnSignedIn = async context =>
    {
        await HandlePostLoginRedirect(context);
    };
});

var app = builder.Build();

// ================== SEED ROLES & ADMIN ==================
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roleNames = { "Admin", "User" };

    foreach (var role in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Seed default Admin
    var adminEmail = "admin@dm.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@123");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

// Post-login redirect handler
async Task HandlePostLoginRedirect(CookieSignedInContext context)
{
    var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
    var user = await userManager.GetUserAsync(context.Principal);

    if (user != null)
    {
        if (await userManager.IsInRoleAsync(user, "Admin"))
        {
            context.HttpContext.Response.Redirect("/Admin/Home/Index");
        }
        else
        {
            context.HttpContext.Response.Redirect("/Landing/Welcome");
        }
    }
}

// Alternative approach: Keep the endpoint for manual redirects
app.MapGet("/postlogin", async (HttpContext context, UserManager<ApplicationUser> userManager) =>
{
    var user = await userManager.GetUserAsync(context.User);
    if (user != null)
    {
        if (await userManager.IsInRoleAsync(user, "Admin"))
        {
            context.Response.Redirect("/Admin/Home/Index");
        }
        else
        {
            context.Response.Redirect("/Landing/Welcome");
        }
    }
    else
    {
        context.Response.Redirect("/Identity/Account/Login");
    }
});

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Default routes
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Landing}/{action=Welcome}/{id?}");

app.MapRazorPages();

app.Run();