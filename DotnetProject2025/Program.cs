using DotnetProject2025.Models;
using DotnetProject2025.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

var builder = WebApplication.CreateBuilder(args);

// Thông tin xác thực Firebase Admin
var firebaseSettings = new JObject
{
    { "type", "service_account" },
    { "project_id", "dotnetproject2025-62899" },
    { "private_key_id", "8b5ebcee0349ad430bfc5693c5423748933a08ab" },
    { "private_key", "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC5QoeXmP60xvlA\ntfCZiB7wD01gHGLDt5Mc8XJ0Q+SYAjNH0g1Fie3Oh6zW+dyyR0t2sQ9UrMw+bh1Z\nKmEzStHO8sTUDcVJPj0RyDDrW9joDgOKlDaJKy1kZhw3yc0g0vmxNPh3x1J9vRNd\nKKNPKBLKA/PLzmRl0pwy6aUN2KkfYRsjasQS14k7rGd3LxJO2V37/Dmqxje4pvmq\nfOklnrpSS3i/O3Y1FYsIXP3tO/t8uSvg7/PtLKVOrimUbCVQZ6f4MNVFGeqRB+1A\n5kgG9p+GEe4lkfFgtst+G87Zw9yebJifQVmlWwJWVWvMXETctH1N+5HG9GBzIUQA\nNJCHA6lxAgMBAAECggEACrzS7BP3i1XPA5OoBtYPaP2SEASFKu/cbHyIFDoNKUxT\nWZHQqBum92YAvBMOWhzswEvh5hf/3Ix4Uf81VAo9n9+NTfcgXK7pTjsKrtGLgGOz\ni9gsUwX4Iw4SC5+8Lm9b/ovh7NtWKiiH00n0MTXS5PeWrVTgl9A9smu9X9CTeNiH\n99lt5uvmUn0uCIWE4mqhkD9Q2kJaxUD8kT/H/ceemgz1GmmH2hSF+18iE04tqvQ7\nAOMScqXOAYgJXrYWIHJj4SRHb8K+4Ce2lzA9BJoo77pb9aiRmM0gE8NPBV16EoMd\n7+HjrmqoMtOff8iqOG9JjahRsIbzTGK92GO7D4PI8wKBgQD9WWrh82f23hULvHZY\nnkIA+viMPDIvI7+PANzCnfV0s5vuvr01keMDHNQbqRW3Y2xr6aExpJ1un20NQVXY\nbIf5xgFHW+9puyEaEBVUwPwDPSjU+XtE9A72lDXfCIQQjgZq+hso6jM/b8TIFXIm\nb3LxDU7hCWh5ygGD9zAV70veowKBgQC7Mr0Cr0Up8crynP8xdIHu2uB9k5BtBtFj\n+X+3oJcEERWzFoW3LYi5mOaRBpEoBRaRL5xxXGPm5mFu5oREKQUYrZNLL+KEJQR2\nU7RlA2Fz45iMc0F8YWtaVGPYYsn2DUlE7h2xvPxihuBHSdu6z2qXnMdJvMuzwTcN\nobrAiDo82wKBgBHaHSfyyN6iY4VCMGyqKih7hswK6dZchIEFJIkqLEOe7Fv0YMzH\njOGJRID2dytG/DrWaZ9f5CAJZ9vJQM1RSHuXMjvfBI3Eu95kR7yDJL7trlTjh4Yi\nVHOXmfwvU27/MwCSwxss9ZOAwrL+n96Jd+X4dBhzW0NJsrWj02WYSRZVAoGBAKsV\nIOauNmCyKU93qkaRexpHkTlTPoBV5dIfuNiifMHrt1A3+jbB953fnoDWZp0ToV9J\nCwoTbP4eeNd491KvxoeaQu5JMhShHCXkNA7JeywFymo5/5RkGE+yppmhF4C2/cpw\ncFT7KhIFwD2Gw7lETN+JGh1K3URIBj+AGoRRPCBXAoGAY23+wdf3Wtlq09mm1j7L\ntmUzZO3IQwboyR53r8s9nexWgr9yDrLn966STm9phFqHMka5jZRY+qATdhYm8LDo\nLnTno5yKaZ/fGNJAMvSk4gPySUom3dQ6slqIIkTc8hy24JJ+tVHDaS2aICQ3Khog\nEZf5h7v+cFBy59SEqFfFSSc=\n-----END PRIVATE KEY-----\n" },
    { "client_email", "firebase-adminsdk-r4xcc@dotnetproject2025-62899.iam.gserviceaccount.com" },
    { "client_id", "107400432336177242980" },
    { "auth_uri", "https://accounts.google.com/o/oauth2/auth" },
    { "token_uri", "https://oauth2.googleapis.com/token" },
    { "auth_provider_x509_cert_url", "https://www.googleapis.com/oauth2/v1/certs" },
    { "client_x509_cert_url", "https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-r4xcc%40dotnetproject2025-62899.iam.gserviceaccount.com" },
    {"universe_domain", "googleapis.com" }
};

// Khởi tạo Firebase Admin SDK
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromJson(firebaseSettings.ToString())
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.CallbackPath = new PathString("/signin-google");
})
.AddCookie();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:5001")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Credential/Login";
    options.LogoutPath = "/Credential/Logout";
    options.Cookie.Name = "DotnetProject2025AuthCookie";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

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

app.Use(async (context, next) =>
{
    context.Response.Headers.Remove("Cross-Origin-Opener-Policy");
    context.Response.Headers.Remove("Cross-Origin-Embedder-Policy");
    context.Response.Headers.Remove("Cross-Origin-Resource-Policy");
    await next();
});

app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "seedData",
    pattern: "{controller=Home}/{action=SeedData}");
app.Run();
