using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace simple_artifacterp_back.Models
{
    public class SystemConfiguration
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // NombreComercial
        public string? CommercialName { get; set; }

        // Logo
        public string? Logo { get; set; }

        // Fondo
        public string? Background { get; set; }

        // FechaActualizacion
        public DateTime UpdatedAt { get; set; }
    }
}
