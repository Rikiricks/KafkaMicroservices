using Conusmer.Service.Models;

namespace Conusmer.Service.Services
{
    public interface IEmployeeService
    {
        public Task<List<EmployeeReport>> EmployeeListAsync();
        public Task<EmployeeReport> GetEmployeeDetailByIdAsync(string employeeId);
        public Task AddEmployeeAsync(EmployeeReport employeeReport);
        public Task UpdateEmployeeAsync(string employeeId, EmployeeReport employeeReport);
        public Task DeleteEmployeeAsync(string employeeId);
    }
}
