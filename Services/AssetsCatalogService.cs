using Amazon.S3;
using Amazon.S3.Model;
using simple_artifacterp_back.DTOs;
using simple_artifacterp_back.Models;
using simple_artifacterp_back.Repositories;

namespace simple_artifacterp_back.Services
{
    public class AssetsCatalogService
    {
        private readonly AssetsRepository _repository;
        private readonly S3BucketSettings _bucketSettings;
        private readonly IAmazonS3 _s3Client;

        public AssetsCatalogService(AssetsRepository repository, S3BucketSettings bucketSettings, IAmazonS3 s3Client)
        {
            _repository = repository;
            _bucketSettings = bucketSettings;
            _s3Client = s3Client;
        }

        public async Task<List<AssetsResponse>> GetAllAsync()
        {
            var assets = await _repository.FindAllAsync();
            var tasks = assets.Select(MapToResponseAsync);
            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }

        public async Task<AssetsResponse?> GetByIdAsync(string id)
        {
            var asset = await _repository.FindByIdAsync(id);
            return asset == null ? null : await MapToResponseAsync(asset);
        }

        public async Task<AssetsResponse> CreateAsync(AssetsRequest request)
        {
            var asset = new Assets
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Image = request.Image,
                Electricity = request.Electricity,
                WearType = request.WearType
            };

            await _repository.InsertAsync(asset);
            return await MapToResponseAsync(asset);
        }

        public async Task<AssetsResponse> CreateWithImageAsync(AssetsRequest request, string imageKey)
        {
            var asset = new Assets
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Image = imageKey,
                Electricity = request.Electricity,
                WearType = request.WearType
            };

            await _repository.InsertAsync(asset);
            return await MapToResponseAsync(asset);
        }

        public async Task<AssetsResponse?> UpdateAsync(string id, AssetsRequest request)
        {
            var asset = await _repository.FindByIdAsync(id);
            if (asset == null)
            {
                return null;
            }

            asset.Name = request.Name;
            asset.Description = request.Description;
            asset.Price = request.Price;
            asset.Image = request.Image;
            asset.Electricity = request.Electricity;
            asset.WearType = request.WearType;

            await _repository.UpdateAsync(asset);
            return await MapToResponseAsync(asset);
        }

        public async Task<bool> DeleteAsync(string id, bool forceDelete)
        {
            var asset = await _repository.FindByIdAsync(id);
            if (asset == null)
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

        private async Task<AssetsResponse> MapToResponseAsync(Assets asset)
        {
            var imageUrl = await BuildFileUrlAsync(asset.Image);
            return new AssetsResponse
            {
                Id = asset.Id,
                Name = asset.Name,
                Description = asset.Description,
                Price = asset.Price,
                Image = imageUrl,
                Electricity = asset.Electricity,
                WearType = asset.WearType
            };
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
