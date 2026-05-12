using simple_artifacterp_back.Enums;

namespace simple_artifacterp_back.Models
{
    public class Quotation
    {
        // CotizacionId
        public int QuotationId { get; set; }

        // Titulo
        public string? Title { get; set; }

        // Fecha
        public DateTime Date { get; set; }

        // ClienteId (Opcional)
        public int? ClientId { get; set; }

        // Descripcion
        public string? Description { get; set; }

        // Estado
        public string? Status { get; set; }

        // TipoProducto
        public ProductType ProductType { get; set; }
    }
}
