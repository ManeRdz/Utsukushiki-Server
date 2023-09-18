using server.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddJsonFile("appsettings.json");
var key = builder.Configuration.GetSection("Settings").GetSection("Key").ToString();
var keyBytes = Encoding.UTF8.GetBytes(key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,

    };
});

builder.Services.AddControllers();
builder.Services.AddDbContext<EcommerceDbContext>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Utsukushiki home store API", Version = "v1" });

});

var app = builder.Build();

app.UseSwagger(c =>
{
    c.SerializeAsV2 = true;
});
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Utsukushiki home store API v1");
});
app.UseCors(options =>
{
    options.WithOrigins("http://localhost:5173");
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowCredentials();
});

app.UseHttpsRedirection();

app.UseCookiePolicy();

app.Use(async (context, next) =>
{
    
    if (context.Request.Cookies.TryGetValue("sessionToken", out var sessionToken))
    {
        context.Request.Headers.Add("Authorization", "Bearer " + sessionToken);
    }

    await next();
});

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
