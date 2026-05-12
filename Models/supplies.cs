using simple_artifacterp_back.Enums;

namespace simple_artifacterp_back.Models
{
    public class Supplies
    {
        // InsumosId
        public int SuppliesId { get; set; }

        // Tipo
        public SupplyType Type { get; set; }

        // Nombre
        public string? Name { get; set; }

        // Color
        public SupplyColor Color { get; set; }

        // Marca
        public string? Brand { get; set; }

        // UltimoCosto
        public decimal? LastCost { get; set; }

        // Descripcion
        public string? Description { get; set; }

        // Activo
        public bool IsActive { get; set; }

        // Impuesto (No usar)
        public decimal? Tax { get; set; }

        // UnidadMedidaId
        public int UnitsMeasurementId { get; set; }
    }
}
