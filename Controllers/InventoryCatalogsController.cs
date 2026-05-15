using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using simple_artifacterp_back.DTOs;
using simple_artifacterp_back.Models;
using simple_artifacterp_back.Services;

namespace simple_artifacterp_back.Controllers
{
    [ApiController]
    [Route("api/inventory/catalogs")]
    public class InventoryCatalogsController : ControllerBase
    {
        private readonly AssetsCatalogService _assetsService;
        private readonly SuppliesCatalogService _suppliesService;
        private readonly UnitsMeasurementCatalogService _unitsService;
        private readonly IAmazonS3 _s3Client;
        private readonly S3BucketSettings _bucketSettings;

        public InventoryCatalogsController(
            AssetsCatalogService assetsService,
            SuppliesCatalogService suppliesService,
            UnitsMeasurementCatalogService unitsService,
            IAmazonS3 s3Client,
            S3BucketSettings bucketSettings)
        {
            _assetsService = assetsService;
            _suppliesService = suppliesService;
            _unitsService = unitsService;
            _s3Client = s3Client;
            _bucketSettings = bucketSettings;
        }

        [HttpGet("assets")]
        public async Task<IActionResult> GetAssets()
        {
            var assets = await _assetsService.GetAllAsync();
            return Ok(assets);
        }

        [HttpGet("assets/{id}")]
        public async Task<IActionResult> GetAssetById(string id)
        {
            var asset = await _assetsService.GetByIdAsync(id);
            return asset == null ? NotFound() : Ok(asset);
        }

        [HttpPost("assets")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateAsset([FromForm] AssetsRequest request)
        {
            if (request.ImageFile != null)
            {
                if (request.ImageFile.ContentType == null || !request.ImageFile.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest("Solo se permiten imágenes.");
                }

                var safeName = (request.Name ?? "asset").Trim().Replace(" ", "-");
                var type = request.WearType.ToString().ToLowerInvariant();
                var dateStamp = DateTime.UtcNow.ToString("yyyyMMdd");
                var key = $"images/assets/{safeName}-{type}-{dateStamp}-{Guid.NewGuid()}";

                await using var stream = request.ImageFile.OpenReadStream();
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketSettings.BucketName,
                    Key = key,
                    InputStream = stream,
                    ContentType = request.ImageFile.ContentType
                };

                await _s3Client.PutObjectAsync(putRequest);
                var assetWithImage = await _assetsService.CreateWithImageAsync(request, key);
                return Ok(assetWithImage);
            }

            var asset = await _assetsService.CreateAsync(request);
            return Ok(asset);
        }

        [HttpPut("assets/{id}")]
        public async Task<IActionResult> UpdateAsset(string id, [FromBody] AssetsRequest request)
        {
            var asset = await _assetsService.UpdateAsync(id, request);
            return asset == null ? NotFound() : Ok(asset);
        }

        [HttpDelete("assets/{id}")]
        public async Task<IActionResult> DeleteAsset(string id, [FromBody] DeleteRequest? request)
        {
            try
            {
                var deleted = await _assetsService.DeleteAsync(id, request?.ForceDelete ?? false);
                return deleted ? Ok() : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("supplies")]
        public async Task<IActionResult> GetSupplies()
        {
            var supplies = await _suppliesService.GetAllAsync();
            return Ok(supplies);
        }

        [HttpGet("supplies/{id}")]
        public async Task<IActionResult> GetSupplyById(string id)
        {
            var supply = await _suppliesService.GetByIdAsync(id);
            return supply == null ? NotFound() : Ok(supply);
        }

        [HttpPost("supplies")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateSupply([FromForm] SuppliesRequest request)
        {
            if (request.ImageFile != null)
            {
                if (request.ImageFile.ContentType == null || !request.ImageFile.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest("Solo se permiten imágenes.");
                }

                var safeName = (request.Name ?? "supply").Trim().Replace(" ", "-");
                var type = request.Type.ToString().ToLowerInvariant();
                var dateStamp = DateTime.UtcNow.ToString("yyyyMMdd");
                var key = $"images/supplies/{safeName}-{type}-{dateStamp}-{Guid.NewGuid()}";

                await using var stream = request.ImageFile.OpenReadStream();
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketSettings.BucketName,
                    Key = key,
                    InputStream = stream,
                    ContentType = request.ImageFile.ContentType
                };

                await _s3Client.PutObjectAsync(putRequest);
                var supplyWithImage = await _suppliesService.CreateWithImageAsync(request, key);
                return Ok(supplyWithImage);
            }

            var supply = await _suppliesService.CreateAsync(request);
            return Ok(supply);
        }

        [HttpPut("supplies/{id}")]
        public async Task<IActionResult> UpdateSupply(string id, [FromBody] SuppliesRequest request)
        {
            var supply = await _suppliesService.UpdateAsync(id, request);
            return supply == null ? NotFound() : Ok(supply);
        }

        [HttpDelete("supplies/{id}")]
        public async Task<IActionResult> DeleteSupply(string id, [FromBody] DeleteRequest? request)
        {
            try
            {
                var deleted = await _suppliesService.DeleteAsync(id, request?.ForceDelete ?? false);
                return deleted ? Ok() : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("supplies/{id}/image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadSupplyImage(string id, [FromForm] UploadFileRequest uploadRequest)
        {
            if (uploadRequest.File == null || uploadRequest.File.Length == 0)
            {
                return BadRequest("Archivo requerido.");
            }

            if (uploadRequest.File.ContentType == null || !uploadRequest.File.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Solo se permiten imágenes.");
            }

            var supply = await _suppliesService.GetByIdAsync(id);
            if (supply == null)
            {
                return NotFound();
            }

            var safeName = (supply.Name ?? "supply").Trim().Replace(" ", "-");
            var type = supply.Type.ToString().ToLowerInvariant();
            var dateStamp = DateTime.UtcNow.ToString("yyyyMMdd");
            var key = $"images/supplies/{safeName}-{type}-{dateStamp}-{Guid.NewGuid()}";

            await using var stream = uploadRequest.File.OpenReadStream();
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketSettings.BucketName,
                Key = key,
                InputStream = stream,
                ContentType = uploadRequest.File.ContentType
            };

            await _s3Client.PutObjectAsync(putRequest);

            var updated = await _suppliesService.UpdateImageAsync(id, key);
            return Ok(updated);
        }

        [HttpGet("units-measurement")]
        public async Task<IActionResult> GetUnitsMeasurement()
        {
            var units = await _unitsService.GetAllAsync();
            return Ok(units);
        }

        [HttpGet("units-measurement/{id}")]
        public async Task<IActionResult> GetUnitMeasurementById(string id)
        {
            var unit = await _unitsService.GetByIdAsync(id);
            return unit == null ? NotFound() : Ok(unit);
        }

        [HttpPost("units-measurement")]
        public async Task<IActionResult> CreateUnitMeasurement([FromBody] UnitsMeasurementRequest request)
        {
            var unit = await _unitsService.CreateAsync(request);
            return Ok(unit);
        }

        [HttpPut("units-measurement/{id}")]
        public async Task<IActionResult> UpdateUnitMeasurement(string id, [FromBody] UnitsMeasurementRequest request)
        {
            var unit = await _unitsService.UpdateAsync(id, request);
            return unit == null ? NotFound() : Ok(unit);
        }

        [HttpDelete("units-measurement/{id}")]
        public async Task<IActionResult> DeleteUnitMeasurement(string id, [FromBody] DeleteRequest? request)
        {
            try
            {
                var deleted = await _unitsService.DeleteAsync(id, request?.ForceDelete ?? false);
                return deleted ? Ok() : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
