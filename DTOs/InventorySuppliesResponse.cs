namespace simple_artifacterp_back.DTOs
{
    public class InventorySuppliesResponse
    {
        public int InventorySuppliesId { get; set; }
        public decimal AvailableQuantity { get; set; }
        public decimal MinimumQuantity { get; set; }
        public decimal CommittedQuantity { get; set; }
        public string? UnitsMeasurementId { get; set; }
        public string? SuppliesId { get; set; }
        public string? LastDispatchNumber { get; set; }
    }
}
