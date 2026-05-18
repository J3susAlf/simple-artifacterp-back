using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using simple_artifacterp_back.Enums;

namespace simple_artifacterp_back.Models
{
    public class Assortment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // SurtidosId
        public int AssortmentId { get; set; }

        // CostoUnitarioCompra
        public decimal UnitaryPurchaseCost { get; set; }

        // Es Pack
        public bool IsPack { get; set; }

        // si es pack, entonces poner la cantidad que tiene el pack
        public int PackQuantity { get; set; }
        
        // CantidadCompra
        public int PurchaseQuantity { get; set; }

        // InsumosId
        public string? SuppliesId { get; set; }

        // UltimaPersonaEdito
        public string? LastEditedByName { get; set; }

        //status del surtido
        public AssortmentStatus? Status { get; set; }

        // FechaDeEntrega
        public DateTime? DeliveryDate { get; set; }
    }
}
