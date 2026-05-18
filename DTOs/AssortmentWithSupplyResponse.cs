using simple_artifacterp_back.Enums;

namespace simple_artifacterp_back.DTOs
{
    public class AssortmentWithSupplyResponse
    {
        public int AssortmentId { get; set; }
        public string? SuppliesId { get; set; }
        public decimal UnitaryPurchaseCost { get; set; }
        public bool IsPack { get; set; }
        public int PackQuantity { get; set; }
        public int PurchaseQuantity { get; set; }
        public string? LastEditedByName { get; set; }
        public AssortmentStatus? Status { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public SupplyDetailsResponse? Supply { get; set; }
    }

    public class SupplyDetailsResponse
    {
        public string? Id { get; set; }
        public SupplyType Type { get; set; }
        public string? Name { get; set; }
        public SupplyColor Color { get; set; }
        public string? Brand { get; set; }
        public string? Image { get; set; }
        public decimal? LastCost { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public decimal? Tax { get; set; }
        public string? UnitsMeasurementId { get; set; }
    }
}
