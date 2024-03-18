using Hangfire;
using Hangfire.SqlServer;
using MailService.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Configuración de los cors
builder.Services.AddCors(options => options.AddPolicy(name: "prueba", policy =>
{
    //policy.WithOrigins("https://localhost:7273", "https://localhost:4200").AllowAnyMethod().AllowAnyHeader();
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
}
    ));


var connectionString = builder.Configuration.GetConnectionString("MyDb");
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

// Agregar Hangfire Server para procesar las colas en segundo plano
builder.Services.AddHangfireServer();

builder.Services.AddSingleton<ISendMailService,SendMailService>();
builder.Services.AddSingleton<RecibirRabbit>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
