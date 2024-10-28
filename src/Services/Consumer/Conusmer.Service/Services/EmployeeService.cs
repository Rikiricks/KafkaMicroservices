using Conusmer.Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Conusmer.Service.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IMongoCollection<EmployeeReport> employeeCollection;

        public EmployeeService(IOptions<EmployeeDbSettings> employeeSettings)
        {
            var mongoClient = new MongoClient(employeeSettings.Value.ConnectionString);
            var dataBase = mongoClient.GetDatabase(employeeSettings.Value.DatabaseName);
            employeeCollection = dataBase.GetCollection<EmployeeReport>(employeeSettings.Value.EmployeeCollectionName);
        }
        public async Task<List<EmployeeReport>> EmployeeListAsync()
        {
            return await employeeCollection.Find(_ => true).ToListAsync();
        }

        public async Task DeleteEmployeeAsync(string id)
        {
            await employeeCollection.DeleteOneAsync(a => a.Id == id);
        }

        public async Task AddEmployeeAsync(EmployeeReport employeeReport)
        {
           await employeeCollection.InsertOneAsync(employeeReport);         
        }

        public async Task<EmployeeReport> GetEmployeeDetailByIdAsync(string id)
        {
            return await employeeCollection.FindSync(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateEmployeeAsync(string id, EmployeeReport employeeReport)
        {
            await employeeCollection.ReplaceOneAsync(x => x.Id == id, employeeReport);
        }
    }
}
