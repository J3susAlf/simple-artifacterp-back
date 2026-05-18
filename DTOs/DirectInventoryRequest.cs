namespace simple_artifacterp_back.DTOs
{
    public class DirectInventoryRequest
    {
        public string? SuppliesId { get; set; }
        public decimal AvailableQuantity { get; set; }
        public decimal MinimumQuantity { get; set; }
        public decimal CommittedQuantity { get; set; }
        public string? UnitsMeasurementId { get; set; }
        public decimal LastCost { get; set; }
        public string? LastEditedByName { get; set; }
    }
}
