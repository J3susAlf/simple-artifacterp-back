using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using simple_artifacterp_back.Enums;
using simple_artifacterp_back.Models;

namespace simple_artifacterp_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemManagerController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;
        private readonly IAmazonS3 _s3Client;
        private readonly S3BucketSettings _bucketSettings;
        private readonly IConfiguration _configuration;

        public SystemManagerController(IMongoDatabase database, IAmazonS3 s3Client, S3BucketSettings bucketSettings, IConfiguration configuration)
        {
            _users = database.GetCollection<User>("Users");
            _s3Client = s3Client;
            _bucketSettings = bucketSettings;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email y contraseña requeridos.");
            }

            var existing = await _users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
            if (existing != null)
            {
                return Conflict("El correo ya está registrado.");
            }

            var user = new User
            {
                Email = request.Email,
                UserName = string.IsNullOrWhiteSpace(request.UserName) ? request.Email : request.UserName,
                DisplayName = request.DisplayName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                UserType = request.UserType,
                Role = request.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _users.InsertOneAsync(user);

            return Ok(new { user.Id, user.Email, user.UserName });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Contraseña requerida.");
            }

            var user = await _users.Find(u => u.Email == request.Email || u.UserName == request.UserName).FirstOrDefaultAsync();
            if (user == null || string.IsNullOrWhiteSpace(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Credenciales inválidas.");
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _users.ReplaceOneAsync(u => u.Id == user.Id, user);

            var token = GenerateToken(user);

            return Ok(new { Token = token, user.UserName, user.Email, user.UserType, user.Role });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _users.Find(_ => true)
                .Project(u => new { u.Id, u.UserName, u.Email, u.DisplayName, u.UserType, u.Role })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("profile/{id}")]
        public async Task<IActionResult> UpdateProfile(string id, [FromBody] UpdateProfileRequest request)
        {
            var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(request.DisplayName))
            {
                user.DisplayName = request.DisplayName;
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                user.Email = request.Email;
            }

            await _users.ReplaceOneAsync(u => u.Id == user.Id, user);

            return Ok(new { user.Id, user.DisplayName, user.Email });
        }

        [HttpPut("password/{id}")]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest("Contraseña actual y nueva son requeridas.");
            }

            var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null || string.IsNullOrWhiteSpace(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return Unauthorized("Credenciales inválidas.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _users.ReplaceOneAsync(u => u.Id == user.Id, user);

            return Ok();
        }

        [HttpPost("profile/{id}/photo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadProfilePhoto(string id, [FromForm] UploadFileRequest uploadRequest)
        {
            var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }

            if (uploadRequest.File == null || uploadRequest.File.Length == 0)
            {
                return BadRequest("Archivo requerido.");
            }

            var key = $"profiles/{Guid.NewGuid()}-{uploadRequest.File.FileName}";
            await using var stream = uploadRequest.File.OpenReadStream();

            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketSettings.BucketName,
                Key = key,
                InputStream = stream,
                ContentType = uploadRequest.File.ContentType
            };

            await _s3Client.PutObjectAsync(putRequest);

            user.ProfilePhotoUrl = key;
            await _users.ReplaceOneAsync(u => u.Id == user.Id, user);

            return Ok(new { user.Id, user.ProfilePhotoUrl });
        }

        private string GenerateToken(User user)
        {
            var key = _configuration["Jwt:Key"] ?? "dev-secret-key";
            var issuer = _configuration["Jwt:Issuer"] ?? "simple-artifacterp";
            var audience = _configuration["Jwt:Audience"] ?? "simple-artifacterp";

            var keyBytes = Encoding.UTF8.GetBytes(key);
            if (keyBytes.Length < 16)
            {
                using var sha256 = System.Security.Cryptography.SHA256.Create();
                keyBytes = sha256.ComputeHash(keyBytes);
            }

            var signingKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty),
                new(ClaimTypes.Role, user.Role.ToString()),
                new("userType", user.UserType.ToString()),
                new(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
