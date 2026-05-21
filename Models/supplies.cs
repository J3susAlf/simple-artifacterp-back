using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using simple_artifacterp_back.Enums;

namespace simple_artifacterp_back.Models
{
    public class Supplies
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Tipo
        public SupplyType Type { get; set; }

        // Nombre
        public string? Name { get; set; }

        // Color
        public SupplyColor Color { get; set; }

        // Marca
        public string? Brand { get; set; }
        // Imagen
        public string? Image { get; set; }

        // UltimoCosto
        public decimal? LastCost { get; set; }

        // CantidadBaseCosto
        public decimal? CostQuantity { get; set; }

        // CostoUnitario
        public decimal? UnitCost { get; set; }

        // Descripcion
        public string? Description { get; set; }

        // Activo
        public bool IsActive { get; set; }

        // Impuesto (No usar)
        public decimal? Tax { get; set; }

        // UnidadMedidaId
        public string? UnitsMeasurementId { get; set; }
    }
}
