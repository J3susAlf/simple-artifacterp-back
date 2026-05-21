using simple_artifacterp_back.Enums;

namespace simple_artifacterp_back.DTOs
{
    public class QuotationListItemResponse
    {
        public int QuotationId { get; set; }
        public string? Title { get; set; }
        public DateTime Date { get; set; }
        public QuoteStatus Status { get; set; }
        public ProductType ProductType { get; set; }
        public List<string> Images { get; set; } = new();
        public string? LastEditedByName { get; set; }
    }

    public class QuotationContextResponse
    {
        public QuotationDetailResponse? Quotation { get; set; }
        public List<AssetsResponse> AssetsCatalog { get; set; } = new();
        public List<SuppliesResponse> SuppliesCatalog { get; set; } = new();
        public List<EnumOptionResponse> ProductTypes { get; set; } = new();
        public List<EnumOptionResponse> QuoteStatuses { get; set; } = new();
        public decimal? CostOfElectricity { get; set; }
    }

    public class QuotationDetailResponse
    {
        public QuotationResponse? Quotation { get; set; }
        public QuotationVersionResponse? Version { get; set; }
        public List<QuotationSuppliesResponse> Supplies { get; set; } = new();
        public List<QuotationAssetsResponse> Assets { get; set; } = new();
        public CostBreakdownResponse? CostBreakdown { get; set; }
    }

    public class QuotationResponse
    {
        public int QuotationId { get; set; }
        public string? Title { get; set; }
        public DateTime Date { get; set; }
        public int? ClientId { get; set; }
        public string? Description { get; set; }
        public QuoteStatus Status { get; set; }
        public ProductType ProductType { get; set; }
        public List<string> Images { get; set; } = new();
        public string? LastEditedByName { get; set; }
    }

    public class QuotationVersionResponse
    {
        public int QuotationVersionId { get; set; }
        public int QuotationId { get; set; }
        public string? SubDescription { get; set; }
        public int VersionNumber { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal Profit { get; set; }
        public List<ExtraCostItem> ExtraCosts { get; set; } = new();
        public decimal? ProductTax { get; set; }
        public decimal LaborCost { get; set; }
        public string? LastEditedByName { get; set; }
        public decimal Discount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalCost { get; set; }
    }

    public class QuotationSuppliesResponse
    {
        public int SuppliesQuotationId { get; set; }
        public string? SuppliesId { get; set; }
        public decimal UsageQuantity { get; set; }
        public decimal Cost { get; set; }
        public decimal SubTotal { get; set; }
        public string? LastEditedByName { get; set; }
        public SupplyDetailsResponse? Supply { get; set; }
    }

    public class QuotationAssetsResponse
    {
        public int AssetsQuotationId { get; set; }
        public string? AssetsId { get; set; }
        public decimal UsageQuantity { get; set; }
        public decimal Cost { get; set; }
        public decimal SubTotal { get; set; }
        public AssetDetailsResponse? Asset { get; set; }
    }

    public class AssetDetailsResponse
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public decimal Electricity { get; set; }
        public WearType WearType { get; set; }
    }

    public class CostBreakdownResponse
    {
        public decimal MaterialsCost { get; set; }
        public decimal AssetsCost { get; set; }
        public decimal LaborCost { get; set; }
        public decimal ExtraCosts { get; set; }
        public decimal BaseCost { get; set; }
        public List<SuppliesCostBreakdown> Supplies { get; set; } = new();
        public decimal ProfitMargin { get; set; }
        public decimal Profit { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal ProductTax { get; set; }
        public decimal TotalCost { get; set; }
    }

    public class SuppliesCostBreakdown
    {
        public string? SuppliesId { get; set; }
        public string? Name { get; set; }
        public decimal UsageQuantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Cost { get; set; }
        public decimal SubTotal { get; set; }
    }

    public class EnumOptionResponse
    {
        public int Value { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
