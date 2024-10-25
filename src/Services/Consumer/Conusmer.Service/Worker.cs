using Confluent.Kafka;
using Conusmer.Service.Models;
using System.Text.Json;

namespace Conusmer.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = "kafka1:9092",
                ClientId = "EmployeeConsumer",
                GroupId = "EmployeeConsumerGroup",
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            consumer.Subscribe("employeeTopic");

            _logger.LogInformation("Subscribed the 'employeeTopic'");

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumedData = consumer.Consume(TimeSpan.FromSeconds(5));

                if (consumedData != null)
                {
                    var employee = JsonSerializer.Deserialize<Employee>(consumedData.Message.Value);

                    EmployeeReport employeeReport = new(Guid.NewGuid(), employee!.Id, employee.Name, employee.Surname);
                    _logger.LogInformation($"Consuming {employee}");
                   
                    _logger.LogInformation("Employee Data: {data}", JsonSerializer.Serialize(employeeReport));
                }

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                
                //await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
