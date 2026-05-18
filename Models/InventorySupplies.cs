using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace simple_artifacterp_back.Models
{
    public class InventorySupplies
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // InventarioInsumoId
        public int InventorySuppliesId { get; set; }

        // CantidadDisponible
        public decimal AvailableQuantity { get; set; }

        // CantidadMinima
        public decimal MinimumQuantity { get; set; }

        // CantidadComprometida
        public decimal CommittedQuantity { get; set; }

        // UnidadMedidaId
        public string? UnitsMeasurementId { get; set; }

        // InsumosId
        public string? SuppliesId { get; set; }

        // UltiNoSegSurtido
        public string? LastDispatchNumber { get; set; }
    }
}
