


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Panel1.Classes;
using panel1.Middleware;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Mvc;
using panel1.Classes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddScoped<Connection>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<TokenValidator>(); 

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactConnection", policy =>
    {
        policy.WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});


builder.WebHost.UseUrls("http://192.168.1.51:5116");
builder.Services.AddScoped<InsertMethod>();
builder.Services.AddScoped<DeleteMethod>();

var app = builder.Build();
app.UseCors("ReactConnection");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseTestMiddleware();
app.UseMiddleware<TestMiddleware>();

//app.UseRouting();
//app.Map("/api/Department/GetAllDepartment", config =>
//{
//    config.UseMiddleware<TestMiddleware>();
//    config.UseEndpoints(endpoints =>
//    {
//        endpoints.MapControllers();
//    });
//});
//app.Map("/WeatherForecast", config =>
//{
//    config.UseMiddleware<TestMiddleware>();s
//    config.UseEndpoints(endpoints =>
//    {
//        endpoints.MapControllers();
//    });
//});


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
