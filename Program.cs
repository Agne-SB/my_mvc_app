using MyMvcApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Services registration (MUST come BEFORE builder.Build()) ---
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();
builder.Services.AddHttpClient("AuthApi", client =>
{
    client.BaseAddress = new Uri("https://authorisation-tv7m.onrender.co");
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

// --- Build the app ---
var app = builder.Build();

// --- Middleware configuration ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // ✅ must be before UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Bestilling}/{action=Index}/{id?}");

app.Run();
