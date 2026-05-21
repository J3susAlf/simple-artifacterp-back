using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace simple_artifacterp_back.Models
{
    public class AssetsQuotation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // ActivosCotizacionId
        public int AssetsQuotationId { get; set; }

        // CantidadUso
        public decimal UsageQuantity { get; set; }

        // Costo
        public decimal Cost { get; set; }

        // SubTotal
        public decimal SubTotal { get; set; }

        // ActivosId
        public string? AssetsId { get; set; }

        // CotizacionVersionId
        public int QuotationVersionId { get; set; }
    }
}
