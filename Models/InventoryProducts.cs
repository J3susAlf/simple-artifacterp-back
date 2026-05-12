namespace simple_artifacterp_back.Models
{
    public class InventoryProducts
    {
        // UniqueID
        public int InventoryProductId { get; set; }

        // CantidadDisponible
        public decimal AvailableQuantity { get; set; }

        // CantidadComprometida
        public decimal CommittedQuantity { get; set; }

        // CantidadMinima
        public decimal MinimumQuantity { get; set; }

        // Fecha
        public DateTime Date { get; set; }

        // ProductosId
        public int ProductId { get; set; }
    }
}
