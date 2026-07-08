using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Infrastructure;
using Application;

var builder = WebApplication.CreateBuilder(args);

try
{
    // project dependencies in Clean Architecture pattern
    builder.Services
    .AddApplication()
    .AddInfrastructure();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // api versioning
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;

        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("x-api-version"),
            new MediaTypeApiVersionReader("x-api-version")
        );
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });


    builder.Services.AddSwaggerGen();

    var app = builder.Build();


    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant()
                );
            }

            options.RoutePrefix = string.Empty;
        });
    }

    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
