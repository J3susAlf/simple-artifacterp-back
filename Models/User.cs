using simple_artifacterp_back.Enums;

namespace simple_artifacterp_back.Models
{
    public class User
    {
        // UsuarioId
        public int UserId { get; set; }

        // Correo
        public string? Email { get; set; }

        // HashContrasena
        public string? PasswordHash { get; set; }

        // NombreMostrado
        public string? DisplayName { get; set; }

        // GoogleId
        public string? GoogleId { get; set; }

        // TipoUsuario
        public UserType UserType { get; set; }

        // Activo
        public bool IsActive { get; set; }

        // FechaCreacion
        public DateTime CreatedAt { get; set; }

        // FechaUltimoAcceso
        public DateTime? LastLoginAt { get; set; }
    }
}
