using Microsoft.AspNetCore.Http;

namespace simple_artifacterp_back.DTOs
{
    public class SystemConfigurationUpdateRequest
    {
        public string? CommercialName { get; set; }
        public string? Logo { get; set; }
        public string? Background { get; set; }
        public double? CostOfElectricity { get; set; }
        public IFormFile? BackgroundFile { get; set; }
        public IFormFile? LogoFile { get; set; }
    }
}
