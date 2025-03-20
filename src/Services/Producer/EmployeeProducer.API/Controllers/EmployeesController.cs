using Confluent.Kafka;
using EmployeeProducer.API.Data;
using EmployeeProducer.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
                BootstrapServers = "localhost:9092",
                Acks = Acks.All,
            };

            using (var producer = new ProducerBuilder<string, string>(producerConfig).Build())
            {
                await producer.ProduceAsync("employeeTopic", message);
            }

            return CreatedAtAction(nameof(CreateEmployee), new { id = employee.Id }, employee);
        }

    }
}
