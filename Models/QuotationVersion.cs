using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace simple_artifacterp_back.Models
{
    public class QuotationVersion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // CotizacionVersionId
        public int QuotationVersionId { get; set; }

        // Descripcion
        public string? SubDescription { get; set; }

        // NumeroVersion
        public int VersionNumber { get; set; }

        // MargenGanacia
        public decimal ProfitMargin { get; set; }

        // Ganacia$
        public decimal Profit { get; set; }

        // CostosExtra(json)
        public string? ExtraCostsJson { get; set; }

        // ImpuestoProducto (no usar)
        public decimal? ProductTax { get; set; }

        // CostoTrabajo
        public decimal LaborCost { get; set; }

        // CotizacionId
        public int QuotationId { get; set; }

        // UltimaPersonaEdito
        public string? LastEditedByName { get; set; }
        // Descuento
        public decimal Discount { get; set; }

        // SubTotal
        public decimal SubTotal { get; set; }
        // total
        public decimal TotalCost { get; set; }
    }
}
