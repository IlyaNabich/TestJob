using FluentValidation;
using TestApiTask.Data;
using TestApiTask.Models;
using TestApiTask.Services;
using TestApiTask.Validators;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.Encoder =
            System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });

builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddValidatorsFromAssemblyContaining<ElementsRequestValidator>();
builder.Services.AddScoped<IElementsRepository, ElementsRepository>();
builder.Services.AddScoped<IElementsService, ElementsService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Test API Task",
        Version = "v1",
        Description = "Парсинг HTML по CSS-селектору, поиск email, расшифровка AES256/ECB."
    });
});

var app = builder.Build();

app.UseSwagger(options =>
{
    options.RouteTemplate = "api/swagger/{documentName}/swagger.json";
});
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "api/swagger";
    options.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Test API Task v1");
});

app.UseRouting();
app.MapControllers();
app.Run();
