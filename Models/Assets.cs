using simple_artifacterp_back.Enums;

namespace simple_artifacterp_back.Models
{
    public class Assets
    {
        // ActivosId
        public int AssetsId { get; set; }

        // Nombre
        public string? Name { get; set; }

        // Descripcion
        public string? Description { get; set; }

        // Precio
        public decimal Price { get; set; }

        // Electricidad
        public decimal Electricity { get; set; }

        // TipoDesgaste
        public WearType WearType { get; set; }
    }
}
