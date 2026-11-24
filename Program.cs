
using library.minimalapi.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.InstallServices();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "Library API v1"); });
}

app.UseHttpsRedirection();
app.MapBookEndpoints();

app.Run();
