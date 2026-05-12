namespace simple_artifacterp_back.Models
{
    public class AssetsQuotation
    {
        // ActivosCotizacionId
        public int AssetsQuotationId { get; set; }

        // CantidadUso
        public decimal UsageQuantity { get; set; }

        // Costo
        public decimal Cost { get; set; }

        // SubTotal
        public decimal SubTotal { get; set; }

        // ActivosId
        public int AssetsId { get; set; }

        // CotizacionVersionId
        public int QuotationVersionId { get; set; }
    }
}
