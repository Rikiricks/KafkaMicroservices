using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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

    public record EmployeeReport 
    {
        public EmployeeReport(Guid employeeId, string name, string surname)
        {
            EmployeeId = employeeId;
            Name = name;
            Surname = surname;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("EmployeeId")]
        [BsonRepresentation(BsonType.Binary)]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid EmployeeId { get; init; }

        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("Surname")]
        public string Surname { get; set; }
    }
}
