namespace simple_artifacterp_back.Models
{
    public class Products
    {
        // ProductosId
        public int ProductId { get; set; }

        // Nombre
        public string? Name { get; set; }

        // Descripcion
        public string? Description { get; set; }
        // Imagenes json
        public string? Images { get; set; }

        // Cantidad
        public decimal Quantity { get; set; }

        // Estado
        public string? Status { get; set; }

        // Fecha
        public DateTime Date { get; set; }

        // VentaId
        public int SaleId { get; set; }
    }
}
