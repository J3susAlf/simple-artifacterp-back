namespace simple_artifacterp_back.Models
{
    public class InventorySupplies
    {
        // InventarioInsumoId
        public int InventorySuppliesId { get; set; }

        // CantidadDisponible
        public decimal AvailableQuantity { get; set; }

        // CantidadMinima
        public decimal MinimumQuantity { get; set; }

        // CantidadComprometida
        public decimal CommittedQuantity { get; set; }

        // InsumosId
        public int SuppliesId { get; set; }

        // UltiNoSegSurtido
        public decimal LastDispatchNumber { get; set; }
    }
}
