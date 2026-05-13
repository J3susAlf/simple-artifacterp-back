using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using simple_artifacterp_back.Enums;

namespace simple_artifacterp_back.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Correo
        public string? Email { get; set; }

        // HashContrasena
        public string? PasswordHash { get; set; }

        // NombreUsuario
        public string? UserName { get; set; }

        // NombreMostrado
        public string? DisplayName { get; set; }

        // GoogleId
        public string? GoogleId { get; set; }

        // FotoPerfil
        public string? ProfilePhotoUrl { get; set; }

        // TipoUsuario
        public UserType UserType { get; set; }

        // Rol
        public UserRole Role { get; set; }

        // Activo
        public bool IsActive { get; set; }

        // FechaCreacion
        public DateTime CreatedAt { get; set; }

        // FechaUltimoAcceso
        public DateTime? LastLoginAt { get; set; }
    }
}
