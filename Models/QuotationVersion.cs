namespace simple_artifacterp_back.Models
{
    public class QuotationVersion
    {
        // CotizacionVersionId
        public int QuotationVersionId { get; set; }

        // Descripcion
        public string? Description { get; set; }

        // NumeroVersion
        public int VersionNumber { get; set; }

        // MargenGanacia
        public decimal ProfitMargin { get; set; }

        // Ganacia$
        public decimal Profit { get; set; }

        // CostosExtra(json)
        public string? ExtraCostsJson { get; set; }

        // ImpuestoProducto (no usar)
        public decimal? ProductTax { get; set; }

        // CostoTrabajo
        public decimal LaborCost { get; set; }

        // CotizacionId
        public int QuotationId { get; set; }
    }
}
