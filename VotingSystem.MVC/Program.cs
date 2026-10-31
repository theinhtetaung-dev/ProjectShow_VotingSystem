using Microsoft.AspNetCore.Authentication.Cookies;
using VotingSystem.MVC.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
    options.Filters.Add<SessionAuthenticationFilter>());
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.IdleTimeout = TimeSpan.FromHours(8);
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SessionAuthenticationFilter>();
builder.Services.AddTransient<ApiAuthorizationHandler>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
builder.Services.AddAuthorization(options =>
{
    // New MVC actions are protected unless they explicitly use [AllowAnonymous].
    options.FallbackPolicy = options.DefaultPolicy;
});
builder.Services.AddHttpClient("VotingSystemApi", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiSettings:BaseUrl"]
        ?? throw new InvalidOperationException("ApiSettings:BaseUrl is not configured."));
}).AddHttpMessageHandler<ApiAuthorizationHandler>();
builder.Services.AddHttpClient("VotingSystemAuthApi", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiSettings:BaseUrl"]
        ?? throw new InvalidOperationException("ApiSettings:BaseUrl is not configured."));
});

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

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
