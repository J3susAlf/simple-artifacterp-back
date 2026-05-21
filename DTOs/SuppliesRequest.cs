using Microsoft.AspNetCore.Http;
using simple_artifacterp_back.Enums;

namespace simple_artifacterp_back.DTOs
{
    public class SuppliesRequest
    {
        public SupplyType Type { get; set; }
        public string? Name { get; set; }
        public SupplyColor Color { get; set; }
        public string? Brand { get; set; }
        public string? Image { get; set; }
        public IFormFile? ImageFile { get; set; }
        public decimal? LastCost { get; set; }
        public decimal? CostQuantity { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public decimal? Tax { get; set; }
        public string? UnitsMeasurementId { get; set; }
    }
}
