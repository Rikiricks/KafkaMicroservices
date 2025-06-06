using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using EmployeeProducer.API.Data;
using EmployeeProducer.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using EmployeeMessage = EmployeeProducer.API.Protos.Employee;

namespace EmployeeProducer.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController(EmployeeDbContext dbContext, ILogger<EmployeesController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            logger.LogInformation("Requesting all employees");
            return await dbContext.Employees.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
        {
            
            dbContext.Employees.Add(employee);
            await dbContext.SaveChangesAsync();

            var message = new Message<string, string>()
            {
                Key = employee.Id.ToString(),
                Value = JsonSerializer.Serialize(employee)
            };

            var producerConfig = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092", //"broker:29092",
                Acks = Acks.All,
            };

            using (var producer = new ProducerBuilder<string, string>(producerConfig).Build())
            {
                await producer.ProduceAsync("employeeTopic", message);
            }

            return CreatedAtAction(nameof(CreateEmployee), new { id = employee.Id }, employee);
        }

        [HttpPost("SendEmployees")]
        public async Task<ActionResult<Employee>> SendEmployee(Employee employee)
        {
            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = "http://localhost:9081" // Adjust for your schema registry
            };

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = "localhost:9092"
            };

            using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);

            using var producer = new ProducerBuilder<string, EmployeeMessage>(producerConfig)
            .SetValueSerializer(new ProtobufSerializer<EmployeeMessage>(schemaRegistry))
            .Build();

            var empMsj= new EmployeeMessage { Id = employee.Id.ToString(), Name = employee.Name, SurName = employee.Surname };

            var result = await producer.ProduceAsync("sendEmployeeTopic", new Message<string, EmployeeMessage>
            {
                Key = empMsj.Id.ToString(),
                Value = empMsj
            });

            Console.WriteLine($"Produced message to: {result.TopicPartitionOffset}");
            return Ok(employee);
        }



    }
}
