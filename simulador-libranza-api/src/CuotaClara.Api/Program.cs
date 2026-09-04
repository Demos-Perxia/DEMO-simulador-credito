using CuotaClara.Api.Endpoints;
using CuotaClara.Application.Catalogs;
using CuotaClara.Application.Simulations;
using CuotaClara.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<CatalogService>();
builder.Services.AddScoped<SimulationService>();
builder.Services.AddCors(options => options.AddPolicy("LocalWeb", policy => policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:4200"]).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseProblemDetails();
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("LocalWeb");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapCuotaClaraEndpoints();
app.Run();

public partial class Program;
