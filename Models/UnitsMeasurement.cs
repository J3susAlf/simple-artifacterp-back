using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace simple_artifacterp_back.Models
{
    public class UnitsMeasurement
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Nombre
        public string? Name { get; set; }

        // Simbolo
        public string? Symbol { get; set; }

        // Tipo
        public string? Type { get; set; }
    }
}
