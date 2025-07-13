using System.Text.Json.Serialization;
using ProjApp.Usage;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    var devAppSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.Development.json");
    builder.Configuration.AddJsonFile(devAppSettingsPath, optional: false);
}
else
{
    var appSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
    builder.Configuration.AddJsonFile(appSettingsPath, optional: false);
}

builder.Configuration.AddUserSecrets<ProjApp.Settings.EmptySettings>(optional: false);
var connectionString = builder.Configuration.GetConnectionString("default");

if (string.IsNullOrEmpty(connectionString)) throw new Exception("No connection string found.");

var assemblies = new string?[]
    {
        typeof(ProjApp.Mapping.SuccessInitialVerificationDto).Assembly.FullName,
        typeof(WebAPI.Controllers.Requests.AddDeviceTypeRequest).Assembly.FullName
    }
    .Select(name => name ?? throw new InvalidOperationException("No assembly name"))
    .ToArray();

builder.Services.RegisterProjectDI(connectionString, assemblies);
builder.Services.AddLogging(cfg =>
{
    cfg.ClearProviders();
    cfg.AddConfiguration(builder.Configuration.GetSection("Logging"));
    cfg.AddConsole();
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("DevelopmentPolicy", policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });
}

builder.Services.AddControllers()
    .AddJsonOptions(cfg => cfg.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(cfg => cfg.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" }));
builder.Services.AddOpenApiDocument();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors("DevelopmentPolicy");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
