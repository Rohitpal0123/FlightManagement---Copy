using FlightManagement.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllersWithViews();
// Register the FlightManagementContext with the DI container.
builder.Services.AddDbContext<FlightManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//auth1
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {

        options.LoginPath = "/Login"; 
        options.LogoutPath = "/Logout"; 
        options.AccessDeniedPath = "/Home/AccessDenied"; 
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;


                return context.Response.WriteAsync("Access denied. Please log in to continue.");
            }
        };

    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
});

builder.Services.AddControllers();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true; // Security measure
    options.Cookie.IsEssential = true;
});



var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();
Console.WriteLine("Hit 1");
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "Hello World");

app.Run();