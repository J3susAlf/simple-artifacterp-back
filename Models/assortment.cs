namespace simple_artifacterp_back.Models
{
    public class Assortment
    {
        // SurtidosId
        public int AssortmentId { get; set; }

        // CostoTotalCompra
        public decimal TotalPurchaseCost { get; set; }

        // CantidadCompra
        public int PurchaseQuantity { get; set; }

        // InsumosId
        public int SuppliesId { get; set; }
    }
}
