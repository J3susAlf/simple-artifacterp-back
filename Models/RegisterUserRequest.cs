using simple_artifacterp_back.Enums;

namespace simple_artifacterp_back.Models
{
    public class RegisterUserRequest
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? DisplayName { get; set; }
        public UserType UserType { get; set; }
        public UserRole Role { get; set; }
    }
}
