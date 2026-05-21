using Microsoft.AspNetCore.Http;
using simple_artifacterp_back.Enums;

namespace simple_artifacterp_back.DTOs
{
    public class QuotationUpsertRequest
    {
        public string? PayloadJson { get; set; }
        public List<IFormFile>? ImageFiles { get; set; }
    }

    public class QuotationUpsertPayload
    {
        public string? Title { get; set; }
        public DateTime? Date { get; set; }
        public int? ClientId { get; set; }
        public string? Description { get; set; }
        public QuoteStatus Status { get; set; }
        public ProductType ProductType { get; set; }
        public List<string>? Images { get; set; }
        public string? LastEditedByName { get; set; }
        public QuotationVersionInput? Version { get; set; }
        public List<QuotationSuppliesInput>? Supplies { get; set; }
        public List<QuotationAssetsInput>? Assets { get; set; }
    }

    public class QuotationVersionInput
    {
        public string? SubDescription { get; set; }
        public int? VersionNumber { get; set; }
        public decimal ProfitMargin { get; set; }
        public List<ExtraCostItem>? ExtraCosts { get; set; }
        public decimal? ProductTax { get; set; }
        public decimal LaborCost { get; set; }
        public decimal Discount { get; set; }
        public string? LastEditedByName { get; set; }
    }

    public class QuotationSuppliesInput
    {
        public string? SuppliesId { get; set; }
        public decimal UsageQuantity { get; set; }
    }

    public class QuotationAssetsInput
    {
        public string? AssetsId { get; set; }
        public decimal UsageQuantity { get; set; }
    }

    public class ExtraCostItem
    {
        public string? Type { get; set; }
        public decimal Cost { get; set; }
    }
}
