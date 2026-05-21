using Amazon.S3;
using Amazon.S3.Model;
using simple_artifacterp_back.DTOs;
using simple_artifacterp_back.Models;
using simple_artifacterp_back.Repositories;

namespace simple_artifacterp_back.Services
{
    public class SuppliesCatalogService
    {
        private readonly SuppliesRepository _repository;
        private readonly S3BucketSettings _bucketSettings;
        private readonly IAmazonS3 _s3Client;

        public SuppliesCatalogService(SuppliesRepository repository, S3BucketSettings bucketSettings, IAmazonS3 s3Client)
        {
            _repository = repository;
            _bucketSettings = bucketSettings;
            _s3Client = s3Client;
        }

        public async Task<List<SuppliesResponse>> GetAllAsync()
        {
            var supplies = await _repository.FindAllAsync();
            var tasks = supplies.Select(MapToResponseAsync);
            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }

        public async Task<SuppliesResponse?> GetByIdAsync(string id)
        {
            var supply = await _repository.FindByIdAsync(id);
            return supply == null ? null : await MapToResponseAsync(supply);
        }

        public async Task<SuppliesResponse> CreateAsync(SuppliesRequest request)
        {
            var supply = new Supplies
            {
                Type = request.Type,
                Name = request.Name,
                Color = request.Color,
                Brand = request.Brand,
                Image = request.Image,
                LastCost = request.LastCost,
                CostQuantity = request.CostQuantity,
                Description = request.Description,
                IsActive = request.IsActive,
                Tax = request.Tax,
                UnitsMeasurementId = request.UnitsMeasurementId
            };

            NormalizeCosts(supply);

            await _repository.InsertAsync(supply);
            return await MapToResponseAsync(supply);
        }

        public async Task<SuppliesResponse> CreateWithImageAsync(SuppliesRequest request, string imageKey)
        {
            var supply = new Supplies
            {
                Type = request.Type,
                Name = request.Name,
                Color = request.Color,
                Brand = request.Brand,
                Image = imageKey,
                LastCost = request.LastCost,
                CostQuantity = request.CostQuantity,
                Description = request.Description,
                IsActive = request.IsActive,
                Tax = request.Tax,
                UnitsMeasurementId = request.UnitsMeasurementId
            };

            NormalizeCosts(supply);

            await _repository.InsertAsync(supply);
            return await MapToResponseAsync(supply);
        }

        public async Task<SuppliesResponse?> UpdateAsync(string id, SuppliesRequest request)
        {
            var supply = await _repository.FindByIdAsync(id);
            if (supply == null)
            {
                return null;
            }

            supply.Type = request.Type;
            supply.Name = request.Name;
            supply.Color = request.Color;
            supply.Brand = request.Brand;
            supply.Image = request.Image;
            supply.LastCost = request.LastCost;
            supply.CostQuantity = request.CostQuantity;
            supply.Description = request.Description;
            supply.IsActive = request.IsActive;
            supply.Tax = request.Tax;
            supply.UnitsMeasurementId = request.UnitsMeasurementId;

            NormalizeCosts(supply);

            await _repository.UpdateAsync(supply);
            return await MapToResponseAsync(supply);
        }

        public async Task<SuppliesResponse?> UpdateImageAsync(string id, string imageKey)
        {
            var supply = await _repository.FindByIdAsync(id);
            if (supply == null)
            {
                return null;
            }

            supply.Image = imageKey;
            await _repository.UpdateAsync(supply);
            return await MapToResponseAsync(supply);
        }

        public async Task<bool> DeleteAsync(string id, bool forceDelete)
        {
            var supply = await _repository.FindByIdAsync(id);
            if (supply == null)
            {
                return false;
            }

            var isInUse = false;
            if (isInUse && !forceDelete)
            {
                throw new InvalidOperationException("Registro en uso.");
            }

            await _repository.DeleteAsync(id);
            return true;
        }

        private async Task<SuppliesResponse> MapToResponseAsync(Supplies supply)
        {
            var imageUrl = await BuildFileUrlAsync(supply.Image);
            return new SuppliesResponse
            {
                Id = supply.Id,
                Type = supply.Type,
                Name = supply.Name,
                Color = supply.Color,
                Brand = supply.Brand,
                Image = imageUrl,
                LastCost = supply.LastCost,
                CostQuantity = supply.CostQuantity,
                UnitCost = supply.UnitCost,
                Description = supply.Description,
                IsActive = supply.IsActive,
                Tax = supply.Tax,
                UnitsMeasurementId = supply.UnitsMeasurementId
            };
        }

        private static void NormalizeCosts(Supplies supply)
        {
            if (supply.UnitCost.HasValue && supply.UnitCost.Value > 0)
            {
                return;
            }

            if (supply.LastCost.HasValue
                && supply.CostQuantity.HasValue
                && supply.CostQuantity.Value > 0)
            {
                supply.UnitCost = supply.LastCost.Value / supply.CostQuantity.Value;
            }
        }

        private async Task<string?> BuildFileUrlAsync(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            var bucket = _bucketSettings.BucketName?.Trim('/') ?? string.Empty;
            if (string.IsNullOrWhiteSpace(bucket))
            {
                return key;
            }

            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucket,
                Key = key,
                Expires = DateTime.UtcNow.AddHours(1),
                Verb = HttpVerb.GET
            };

            return await _s3Client.GetPreSignedURLAsync(request);
        }
    }
}
