using Conusmer.Service;
using Conusmer.Service.Services;
using MongoDB.Bson;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<EmployeeDbSettings>(builder.Configuration.GetSection("EmployeeDbSettings"));
builder.Services.AddSingleton<IEmployeeService, EmployeeService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
