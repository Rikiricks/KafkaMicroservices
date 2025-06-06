using Conusmer.Service;
using Conusmer.Service.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<EmployeeDbSettings>(builder.Configuration.GetSection("EmployeeDbSettings"));
builder.Services.AddSingleton<IEmployeeService, EmployeeService>();
//builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<Worker2>();

var host = builder.Build();
host.Run();
