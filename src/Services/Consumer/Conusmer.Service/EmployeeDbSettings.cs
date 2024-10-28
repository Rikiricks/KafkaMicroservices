namespace Conusmer.Service
{
    public class EmployeeDbSettings
    {
        public required string ConnectionString { get; set; }
        public required string DatabaseName { get; set; }
        public required string EmployeeCollectionName { get; set; }
    }
}
