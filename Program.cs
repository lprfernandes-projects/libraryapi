
using library.minimalapi.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.InstallServices();

var app = builder.Build();

app.MapBookEndpoints();

app.Run();
