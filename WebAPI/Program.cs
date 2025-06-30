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

builder.Services.AddLogging(cfg =>
{
    cfg.ClearProviders();
    cfg.AddConfiguration(builder.Configuration.GetSection("Logging"));
    cfg.AddConsole();
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(cfg => cfg.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" }));
builder.Services.RegisterProjectDI(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
