using simple_artifacterp_back.Enums;

namespace simple_artifacterp_back.Models
{
    public class Sale
    {
        // VentaId
        public int SaleId { get; set; }

        // Abonos(Json)
        public string? PaymentsJson { get; set; }

        // Descripcion
        public string? Description { get; set; }

        // TipoVenta
        public string? SaleType { get; set; }

        // Estado
        public SaleStatus Status { get; set; }

        // Ubicacion (puede ser null) json
        public string? LocationJson { get; set; }

        // Fecha
        public DateTime Date { get; set; }

        // CostoTotal
        public decimal TotalCost { get; set; }

        // ClienteId
        public int ClientId { get; set; }

        // CotizacionId
        public int QuotationId { get; set; }

        // UltimaPersonaEdito
        public string? LastEditedByName { get; set; }
    }
}
