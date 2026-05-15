using Microsoft.AspNetCore.Http;
using simple_artifacterp_back.Enums;

namespace simple_artifacterp_back.DTOs
{
    public class AssetsRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public IFormFile? ImageFile { get; set; }
        public decimal Electricity { get; set; }
        public WearType WearType { get; set; }
    }
}
