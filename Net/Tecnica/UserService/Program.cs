
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Prueba.Application.Services.AuthenticationService;
using Prueba.Application.Services.TokenService;
using Prueba.Application.Services;
using System.Text;
using Prueba.Domain.IRepository;
using Prueba.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Prueba.Infrastructure.Repository;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
 
var connectionString = builder.Configuration.GetConnectionString("MyDb");

builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));

//Configuración de los cors
builder.Services.AddCors(options => options.AddPolicy(name: "prueba", policy =>
{
    //policy.WithOrigins("https://localhost:7273", "https://localhost:4200").AllowAnyMethod().AllowAnyHeader();
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
}
    ));

//Implementación de la autentificación con JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
    };
});

//Declaración de los esquemas de servicio

builder.Services.AddSingleton<EnviarRabbit>();
builder.Services.AddSingleton<BcryptPassword>();
builder.Services.AddSingleton<TokenGenerator>();
builder.Services.AddSingleton<RefreshTokenGenerator>();
builder.Services.AddSingleton<RefreshTokenValidator>();
builder.Services.AddScoped<RegistroHub>();
builder.Services.AddScoped<Authenticator>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
