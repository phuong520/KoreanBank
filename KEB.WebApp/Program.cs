using KEB.WebAPI.SignalR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:7101").
                          AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                      });
});

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddHttpClient();
builder.Services.AddHttpClient("MyApiClient", client =>
{
    client.Timeout = TimeSpan.FromMinutes(5); 
});

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "KEB.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddSignalR();
//dd Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;

})

    .AddCookie(options =>
    {
        options.LoginPath = "/Commonweb/Login";
        options.LogoutPath = "/Commonweb/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.Cookie.Name = "KEB.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidIssuer = "JwtIssuer",
//            ValidateAudience = true,
//            ValidAudience = "JwtAudience",
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperSecretJWTKey1234567890abcdef")),
//            ValidateLifetime = true
//        };
//        options.Events = new JwtBearerEvents
//        {
//            OnMessageReceived = context =>
//            {
//                if (context.Request.Cookies.ContainsKey("token"))
//                {
//                    context.Token = context.Request.Cookies["token"];
//                }
//                return Task.CompletedTask;
//            }
//        };
//    });

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else 
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
app.UseSession();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotifyHub>("/notifyHub");
// Add Authentication & Authorization middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Statistics}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "home",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
