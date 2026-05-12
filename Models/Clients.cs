namespace simple_artifacterp_back.Models
{
    public class Clients
    {
        // ClientesId
        public int ClientId { get; set; }

        // Apodo
        public string? Nickname { get; set; }

        // Nombres
        public string? FirstName { get; set; }

        // Apellidos
        public string? LastName { get; set; }

        // Direccion (son null)
        public string? Address { get; set; }

        // Tipo
        public string? Type { get; set; }
    }
}
