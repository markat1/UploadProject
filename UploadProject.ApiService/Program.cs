using UploadProject.ApiService.Api;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.Services.AddAntiforgery();
builder.Services.AddServiceDiscoveryCore();

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseAntiforgery();


// Configure the HTTP request pipeline.
app.UseExceptionHandler();


app.MapDefaultEndpoints();
app.MapUploadApiEndpoints();

app.MapControllers();

app.Run();