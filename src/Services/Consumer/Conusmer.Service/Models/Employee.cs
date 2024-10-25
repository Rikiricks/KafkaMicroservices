namespace Conusmer.Service.Models
{
    public record Employee
    {
        public Employee(Guid id, string name, string surname)
        {
            Id = id;
            Name = name;
            Surname = surname;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }

    public record EmployeeReport : Employee
    {
        public EmployeeReport(Guid id, Guid employeeId, string name, string surname) : base(id, name, surname)
        {
            EmployeeId = employeeId;
        }
        public Guid EmployeeId { get; init; }
    }
}
