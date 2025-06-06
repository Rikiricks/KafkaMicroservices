using Confluent.Kafka;
using Conusmer.Service.Models;
using Conusmer.Service.Services;
using System.Text.Json;

namespace Conusmer.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEmployeeService _employeeService;

        public Worker(ILogger<Worker> logger, IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = "localhost:9092", //"broker:29092",
                ClientId = "EmployeeConsumer",
                GroupId = "EmployeeConsumerGroupNew",
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            consumer.Subscribe("employeeTopic");

            _logger.LogInformation("Subscribed the 'employeeTopic'");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {

                
                var consumedData = consumer.Consume(TimeSpan.FromSeconds(5));

                    if (consumedData != null)
                    {
                        var employee = JsonSerializer.Deserialize<Employee>(consumedData.Message.Value);

                        EmployeeReport employeeReport = new(employee!.Id, employee.Name, employee.Surname);

                        await _employeeService.AddEmployeeAsync(employeeReport);

                        var employees = await _employeeService.EmployeeListAsync();

                        foreach (var item in employees)
                        {
                            _logger.LogInformation("Employee Data: {data}", JsonSerializer.Serialize(employeeReport));
                        }


                    }

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                }
                catch (Exception ex)
                {

                }

                //await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
