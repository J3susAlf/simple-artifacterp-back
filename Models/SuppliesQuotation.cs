namespace simple_artifacterp_back.Models
{
    public class SuppliesQuotation
    {
        // InsumosCotizacionId
        public int SuppliesQuotationId { get; set; }

        // CantidadUso
        public decimal UsageQuantity { get; set; }

        // Costo
        public decimal Cost { get; set; }

        // SubTotal
        public decimal SubTotal { get; set; }

        // InsumoId
        public int SuppliesId { get; set; }

        // CotizacionVersionId
        public int QuotationVersionId { get; set; }

        // UltimaPersonaEdito
        public string? LastEditedByName { get; set; }
    }
}
