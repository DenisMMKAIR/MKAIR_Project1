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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(cfg => cfg.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" }));
builder.Services.RegisterProjectDI(builder.Configuration);

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
