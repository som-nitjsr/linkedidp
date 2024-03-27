using linkedinidp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
  
})
          

                 .AddOpenIdConnect("LinkedIn", "LinkedIn", options =>
                 {
                    
                     options.Authority = "https://www.linkedin.com/oauth/";
                     options.CallbackPath = "/signin-linkedin";
                     options.ClientId = "****";
                     options.ClientSecret = "**";
                     options.ResponseType = "code";
                     options.Scope.Add("openid");
                     options.Scope.Add("profile");
                     options.Scope.Add("email");
                     options.ProtocolValidator.RequireNonce = false;
                     options.Events = new OpenIdConnectEvents
                     {
                         OnAuthorizationCodeReceived = async context =>
                         {
                             // Remove the 'code_verifier' parameter
                             context.TokenEndpointRequest.Parameters.Remove("code_verifier");
                             await Task.CompletedTask;
                         }
                     };
                   
                 });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
