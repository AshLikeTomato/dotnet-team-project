using DotnetProject2025.Models;
using DotnetProject2025.Services;
using Firebase.Database;
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
    { "project_id", "dotnetproject2025" },
    { "private_key_id", "60a1d47d5449c90facb30ccfc8f9d799187621db" },
    { "private_key", "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQCciFqnFP6ulBxS\nAag6kprR/tAoWiP6xWECJ7Zip7qKK+nBK9qj2fMIfLChYuKvH+Tv1l1sxHVkSYA7\nQzXyiTAS/Nvpu+7plEFlO79b8NHS+E2lue+KBlJNJVTYpFPSo5sIBosRNWOn0dBt\nMsMcohpydPyMfwTKkZkbQLMLtxxYE3ckImbHgw6jjX1uhCGfP8XGP5HfZDqTjvl9\n33ifg6FQI6Vj3okf6wvHU9ETDgWwDwiwUujtDH6AD/CTx4syJKGmIU8In/uPCrwl\nA7cTO4III+dwYrS9QslDNoWDf8uwSsBiApdMnJpMS14hYWlH5BeC2YuLIDdoBQtA\nAA4qEsr5AgMBAAECggEAKmfsOyh+aJg6ZeMKCrgt1icUiGIHLkgrpDRS/j4XVJhZ\n7tmtFUeVxh5VkZsi8zOOR4VI8Q72CJG6+hMWudp9mh9Dino92qyt3LmHeWZvyZo4\nGuYY5UkaIckzx68ZAE8PUGjJYwOWOGAoeXTPk+G8jXZw9rTwfNpd4pqwBsP4G0na\nFgnDjBfOCPpYpfbWiz7AWeHofxeaJuoyfPThwVL8AOSeiYraKszWeOD3UpChdbwB\nDfk/PhmXfSV+N9xWnG0LCMcCUNlq1jSuRiPSfRxqF12ueYb0dkBRtSHDRznkz0VP\npeYdO1PH0aJFCvdjFelHz1Wkt9LRV6QkTUbcL5fLQwKBgQDTNzFfKE2gIu1sXVL/\nYJapJVSuwo26xWFmeF//jpu/rzDHdW1LQDHTTIw7SAw9BEGwTlODRofRLm9O0cBf\nPdv7LT/cz3xvP8WExWaP/ycPk4bsZ4smMJjdvnkGgOmL3U3uTPwL4/Z0CN5Iwzwr\nZ3yc89938TB1czjyF0OK/YqgJwKBgQC9uPawHaSOXTT9gONSeYdTtsevaet3zXsb\nJARyrsZPOnb5RGx5JdIh7OQ7h8W8cEnpubXmowJ5J7ZIpOasBNd2cDMp8YJ8jExL\n1/s2XQiZXF8Cs87EyHn2oQQdKBLSGfDu9cpdkJbR8WkFoiOCvKIF/p93spl62aHf\nl8nK7/kP3wKBgQCpRTBSHJCpZ5RzJg+gDp5vjDVjoRPIvOqkHpCA1J9JbXNgRmGC\nzz2fI1e3IrR8Ke8jBoKzUFEEeXxG5J6RNZgYoljt83K8DZWUJEdf25JIT2jwCMNh\npcy82StQ5PtGVeNguV29gcI/l+Pc2GpeNp+NgXx0OV0mo2Ld0XsV5r+0PQKBgQCu\nloiEtzKYQ9OguTv3yJgtBFWr6afjXnc8GZ/yPT2G6UFUz4+WyKQfzhSVZ7MiXp7V\n860d6cI75byMCxuvDuGMP2IF5hZzMNSBMucdNu3T2zofjYHoeqgpujDD9/2h1Hdw\nH0WzZnw0BJjlszhEdfwP0inDh9pPP/kDfnYXR1AGNwKBgH5CPtEDKYempBKHDfNN\nX6iLmbgd9psyotlnbKEp307/3Coj72UU2oqDz2pTXhNggykeTM2fIh8vTWIUlsPo\nGtpJ+/0pYQ+Hs1ebkNBpgccZli208WeT0qnjTt58x5bBMqLgNNcbvp83zO3SDV50\n4zvfhbXbGjhFhgvdheab668p\n-----END PRIVATE KEY-----\n" },
    { "client_email", "firebase-adminsdk-cyuai@dotnetproject2025.iam.gserviceaccount.com" },
    { "client_id", "104278228851371152209" },
    { "auth_uri", "https://accounts.google.com/o/oauth2/auth" },
    { "token_uri", "https://oauth2.googleapis.com/token" },
    { "auth_provider_x509_cert_url", "https://www.googleapis.com/oauth2/v1/certs" },
    { "client_x509_cert_url", "https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-cyuai%40dotnetproject2025.iam.gserviceaccount.com" },
    {"universe_domain", "googleapis.com" }
};

// Khởi tạo Firebase Admin SDK
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromJson(firebaseSettings.ToString())
});
builder.Services.AddSingleton(new FirebaseClient("https://doannet-4a9c2-default-rtdb.asia-southeast1.firebasedatabase.app/"));

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));


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

// function chatbox
builder.Services.AddSignalR();

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


// -------------------------------------------


// Đọc cấu hình SMTP từ appsettings.json



app.Run();
