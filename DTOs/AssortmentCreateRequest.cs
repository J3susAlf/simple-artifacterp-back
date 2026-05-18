namespace simple_artifacterp_back.DTOs
{
    public class AssortmentCreateRequest
    {
        public string? SuppliesId { get; set; }
        public bool IsPack { get; set; }
        public int PackQuantity { get; set; }
        public int PurchaseQuantity { get; set; }
        public decimal UnitaryPurchaseCost { get; set; }
        public bool? ImmediateDelivery { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? LastEditedByName { get; set; }
    }
}
