using Serilog;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Logging;
using Infrastructure.Repositories;
using Presentation.Configurations;
using Presentation.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Logging
SerilogSetup.Configure();
builder.Host.UseSerilog();

// Config
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

//MongoDB
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// MediatR
builder.Services.RegisterMediatR();

// JwT Authentication
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddProblemDetails();

// CORS
builder.Services.ConfigureCors();

// Health Checks
builder.Services.AddHealthChecks();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed MongoDB
using (var scope = app.Services.CreateScope())
{
    var mongoDb = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    await mongoDb.SeedAsync();
}

//Middlewares
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.ConfigureExceptionHandler();
app.ConfigureSecurityHeaders();
app.UseHealthChecks("/health");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserService API V1");
});

// Endpoints
app.RegisterUserEndpoint();


app.Run();
