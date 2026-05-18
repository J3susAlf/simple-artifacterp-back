using simple_artifacterp_back.Enums;

namespace simple_artifacterp_back.DTOs
{
    public class AssortmentResponse
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
    }
}
