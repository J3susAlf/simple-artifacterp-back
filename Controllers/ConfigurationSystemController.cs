using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
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
                    UpdatedAt = DateTime.UtcNow
                };

                await _systemConfigurations.InsertOneAsync(configuration);
                return Ok(configuration);
            }

            return Ok(existing);
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
