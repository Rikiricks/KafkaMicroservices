using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Conusmer.Service.Models;
using System.Text.Json;
using EmployeeMessage = EmployeeProducer.API.Protos.Employee;

namespace Conusmer.Service
{
    public class Worker2 : BackgroundService
    {
        private readonly ILogger<Worker2> _logger;       

        public Worker2(ILogger<Worker2> logger)
        {
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = "http://localhost:9081"
            };

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "employee-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);

            using var consumer = new ConsumerBuilder<string, EmployeeMessage>(consumerConfig)
                .SetValueDeserializer(new ProtobufDeserializer<EmployeeMessage>().AsSyncOverAsync())
                .Build();

            consumer.Subscribe("sendEmployeeTopic");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumedData = consumer.Consume(TimeSpan.FromSeconds(5));

                    if (consumedData != null)
                    {
                        var employee = consumedData.Message.Value;
                        EmployeeReport employeeReport = new(new Guid(employee.Id), employee.Name, employee.SurName);
                        _logger.LogInformation("Employee Data: {data}", JsonSerializer.Serialize(employeeReport));                  
                    }

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Worker2 running at: {time}", DateTimeOffset.Now);
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
