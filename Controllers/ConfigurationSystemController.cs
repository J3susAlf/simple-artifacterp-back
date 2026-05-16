using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using simple_artifacterp_back.DTOs;
using MongoDB.Driver;
using simple_artifacterp_back.Models;

namespace simple_artifacterp_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationSystemController : ControllerBase
    {
        private readonly IMongoCollection<SystemConfiguration> _systemConfigurations;
        private readonly IAmazonS3 _s3Client;
        private readonly S3BucketSettings _bucketSettings;

        public ConfigurationSystemController(IMongoDatabase database, IAmazonS3 s3Client, S3BucketSettings bucketSettings)
        {
            _systemConfigurations = database.GetCollection<SystemConfiguration>("SystemConfiguration");
            _s3Client = s3Client;
            _bucketSettings = bucketSettings;
        }

        [HttpPost("check")]
        public async Task<IActionResult> CheckAndCreateConfiguration()
        {
            var existing = await _systemConfigurations.Find(_ => true).FirstOrDefaultAsync();
            if (existing is null)
            {
                var configuration = new SystemConfiguration
                {
                    CommercialName = "Artifac ERP",
                    Logo = "default-logo.png",
                    Background = "default-background.png",
                    CostOfElectricity = 0,
                    UpdatedAt = DateTime.UtcNow
                };

                await _systemConfigurations.InsertOneAsync(configuration);
                return Ok(configuration);
            }

            return Ok(existing);
        }

        [HttpGet]
        public async Task<IActionResult> GetConfiguration()
        {
            var existing = await _systemConfigurations.Find(_ => true).FirstOrDefaultAsync();
            if (existing == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(existing.Background))
            {
                existing.Background = await GetPresignedUrlAsync(existing.Background);
            }

            if (!string.IsNullOrWhiteSpace(existing.Logo))
            {
                existing.Logo = await GetPresignedUrlAsync(existing.Logo);
            }

            return Ok(existing);
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateConfiguration(string id, [FromForm] SystemConfigurationUpdateRequest request)
        {
            var existing = await _systemConfigurations.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (existing == null)
            {
                return NotFound();
            }

            existing.CommercialName = request.CommercialName;
            existing.Logo = NormalizeObjectKey(request.Logo);
            existing.Background = NormalizeObjectKey(request.Background);
            existing.CostOfElectricity = request.CostOfElectricity;

            if (request.BackgroundFile != null)
            {
                if (request.BackgroundFile.ContentType == null || !request.BackgroundFile.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest("Solo se permiten imágenes.");
                }

                var key = $"system/backgrounds/{Guid.NewGuid()}-{request.BackgroundFile.FileName}";
                await using var stream = request.BackgroundFile.OpenReadStream();

                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketSettings.BucketName,
                    Key = key,
                    InputStream = stream,
                    ContentType = request.BackgroundFile.ContentType
                };

                await _s3Client.PutObjectAsync(putRequest);
                existing.Background = key;
            }

            if (request.LogoFile != null)
            {
                if (!IsLogoContentTypeAllowed(request.LogoFile.ContentType))
                {
                    return BadRequest("Tipo de archivo de logo no permitido.");
                }

                var key = $"system/logos/{Guid.NewGuid()}-{request.LogoFile.FileName}";
                await using var stream = request.LogoFile.OpenReadStream();

                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketSettings.BucketName,
                    Key = key,
                    InputStream = stream,
                    ContentType = request.LogoFile.ContentType
                };

                await _s3Client.PutObjectAsync(putRequest);
                existing.Logo = key;
            }

            existing.UpdatedAt = DateTime.UtcNow;

            await _systemConfigurations.ReplaceOneAsync(c => c.Id == id, existing);
            return Ok(existing);
        }

        private Task<string> GetPresignedUrlAsync(string key)
        {
            if (key.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || key.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(key);
            }

            var bucket = _bucketSettings.BucketName?.Trim('/') ?? string.Empty;
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucket,
                Key = key,
                Expires = DateTime.UtcNow.AddHours(1),
                Verb = HttpVerb.GET
            };

            return _s3Client.GetPreSignedURLAsync(request);
        }

        private string? NormalizeObjectKey(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                return value;
            }

            var bucket = _bucketSettings.BucketName?.Trim('/') ?? string.Empty;
            if (string.IsNullOrWhiteSpace(bucket))
            {
                return value;
            }

            var path = uri.AbsolutePath.TrimStart('/');
            if (path.StartsWith(bucket + "/", StringComparison.OrdinalIgnoreCase))
            {
                return path[(bucket.Length + 1)..];
            }

            return value;
        }

        private static bool IsLogoContentTypeAllowed(string? contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                return false;
            }

            return contentType.Equals("image/png", StringComparison.OrdinalIgnoreCase)
                   || contentType.Equals("image/svg+xml", StringComparison.OrdinalIgnoreCase)
                   || contentType.Equals("image/x-icon", StringComparison.OrdinalIgnoreCase)
                   || contentType.Equals("image/vnd.microsoft.icon", StringComparison.OrdinalIgnoreCase);
        }

        [HttpPost("files")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileRequest uploadRequest)
        {
            if (uploadRequest.File == null || uploadRequest.File.Length == 0)
            {
                return BadRequest("Archivo requerido.");
            }

            var key = $"uploads/{Guid.NewGuid()}-{uploadRequest.File.FileName}";
            await using var stream = uploadRequest.File.OpenReadStream();

            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketSettings.BucketName,
                Key = key,
                InputStream = stream,
                ContentType = uploadRequest.File.ContentType
            };

            await _s3Client.PutObjectAsync(putRequest);

            return Ok(new { Key = key });
        }

        [HttpGet("files")]
        public async Task<IActionResult> ListFiles()
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _bucketSettings.BucketName,
                Prefix = "uploads/"
            };

            var response = await _s3Client.ListObjectsV2Async(request);
            var files = response.S3Objects.Select(obj => new { obj.Key, obj.Size, obj.LastModified });

            return Ok(files);
        }
    }
}
