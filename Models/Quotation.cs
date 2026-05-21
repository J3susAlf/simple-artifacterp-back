using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using simple_artifacterp_back.Enums;

namespace simple_artifacterp_back.Models
{
    public class Quotation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // CotizacionId
        public int QuotationId { get; set; }

        // Titulo
        public string? Title { get; set; }

        // Fecha
        public DateTime Date { get; set; }

        // ClienteId (Opcional)
        public int? ClientId { get; set; }

        // Descripcion
        public string? Description { get; set; }

        // Estado
        public QuoteStatus Status { get; set; }

        // TipoProducto
        public ProductType ProductType { get; set; }

        // Imagenes json
        public string? Images { get; set; }
        // UltimaPersonaEdito
        public string? LastEditedByName { get; set; }
    }
}
