using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Melotrade.Data;
using Melotrade.Models;
using Melotrade.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>(sp =>
{
    var smtpSettings = sp.GetRequiredService<IOptions<SmtpSettings>>().Value;
    return new EmailSender(smtpSettings);
});

builder.Services.AddHttpClient("Yoco", client =>
{
    client.BaseAddress = new Uri("https://payments.yoco.com/");
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", builder.Configuration["Yoco:SecretKey"]);
});

builder.Services.AddScoped<YocoPaymentService>();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true; // Set to true if using email confirmation
})
.AddRoles<IdentityRole>() // Optional, if you want to support Roles
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager<SignInManager<ApplicationUser>>()
.AddDefaultUI(); 

builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
