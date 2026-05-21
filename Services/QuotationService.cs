using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using simple_artifacterp_back.DTOs;
using simple_artifacterp_back.Enums;
using simple_artifacterp_back.Models;
using simple_artifacterp_back.Repositories;
using System.Text.Json;

namespace simple_artifacterp_back.Services
{
    public class QuotationService
    {
        private readonly QuotationRepository _quotationRepository;
        private readonly QuotationVersionRepository _versionRepository;
        private readonly SuppliesQuotationRepository _suppliesQuotationRepository;
        private readonly AssetsQuotationRepository _assetsQuotationRepository;
        private readonly SuppliesRepository _suppliesRepository;
        private readonly AssetsRepository _assetsRepository;
        private readonly IMongoCollection<SystemConfiguration> _systemConfigurations;
        private readonly QuotationPricingService _pricingService;
        private readonly IAmazonS3 _s3Client;
        private readonly S3BucketSettings _bucketSettings;

        public QuotationService(
            QuotationRepository quotationRepository,
            QuotationVersionRepository versionRepository,
            SuppliesQuotationRepository suppliesQuotationRepository,
            AssetsQuotationRepository assetsQuotationRepository,
            SuppliesRepository suppliesRepository,
            AssetsRepository assetsRepository,
            IMongoDatabase database,
            QuotationPricingService pricingService,
            IAmazonS3 s3Client,
            S3BucketSettings bucketSettings)
        {
            _quotationRepository = quotationRepository;
            _versionRepository = versionRepository;
            _suppliesQuotationRepository = suppliesQuotationRepository;
            _assetsQuotationRepository = assetsQuotationRepository;
            _suppliesRepository = suppliesRepository;
            _assetsRepository = assetsRepository;
            _systemConfigurations = database.GetCollection<SystemConfiguration>("SystemConfiguration");
            _pricingService = pricingService;
            _s3Client = s3Client;
            _bucketSettings = bucketSettings;
        }

        public async Task<List<QuotationListItemResponse>> GetListAsync(
            int? year,
            int? month,
            ProductType? productType,
            QuoteStatus? status,
            string? search)
        {
            var quotations = await _quotationRepository.FindAllAsync();
            var filtered = quotations.AsEnumerable();

            if (year.HasValue && month.HasValue)
            {
                filtered = filtered.Where(q => q.Date.Year == year.Value && q.Date.Month == month.Value);
            }

            if (productType.HasValue)
            {
                filtered = filtered.Where(q => q.ProductType == productType.Value);
            }

            if (status.HasValue)
            {
                filtered = filtered.Where(q => q.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                filtered = filtered.Where(q => (q.Title ?? string.Empty)
                    .Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            var results = new List<QuotationListItemResponse>();
            foreach (var quotation in filtered.OrderByDescending(q => q.Date))
            {
                var images = await BuildImageUrlsAsync(ParseImageList(quotation.Images));
                results.Add(new QuotationListItemResponse
                {
                    QuotationId = quotation.QuotationId,
                    Title = quotation.Title,
                    Date = quotation.Date,
                    Status = quotation.Status,
                    ProductType = quotation.ProductType,
                    Images = images,
                    LastEditedByName = quotation.LastEditedByName
                });
            }

            return results;
        }

        public async Task<QuotationContextResponse?> GetContextAsync(int? quotationId)
        {
            var context = await BuildContextAsync();

            if (quotationId.HasValue)
            {
                var detail = await BuildQuotationDetailAsync(quotationId.Value);
                if (detail == null)
                {
                    return null;
                }

                context.Quotation = detail;
            }

            return context;
        }

        public async Task<QuotationContextResponse> CreateAsync(QuotationUpsertPayload payload, List<IFormFile>? imageFiles)
        {
            var detail = await UpsertAsync(null, payload, imageFiles);
            var context = await BuildContextAsync();
            context.Quotation = detail;
            return context;
        }

        public async Task<QuotationContextResponse?> UpdateAsync(int quotationId, QuotationUpsertPayload payload, List<IFormFile>? imageFiles)
        {
            var detail = await UpsertAsync(quotationId, payload, imageFiles);
            if (detail == null)
            {
                return null;
            }

            var context = await BuildContextAsync();
            context.Quotation = detail;
            return context;
        }

        public async Task<List<SuppliesResponse>> SearchSuppliesAsync(string? name)
        {
            var supplies = await _suppliesRepository.FindAllAsync();
            if (!string.IsNullOrWhiteSpace(name))
            {
                supplies = supplies
                    .Where(s => (s.Name ?? string.Empty).Contains(name, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var tasks = supplies.Select(MapSupplyToResponseAsync);
            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }

        public async Task<List<AssetsResponse>> SearchAssetsAsync(string? name)
        {
            var assets = await _assetsRepository.FindAllAsync();
            if (!string.IsNullOrWhiteSpace(name))
            {
                assets = assets
                    .Where(a => (a.Name ?? string.Empty).Contains(name, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var tasks = assets.Select(MapAssetToResponseAsync);
            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }

        private async Task<QuotationDetailResponse?> UpsertAsync(int? quotationId, QuotationUpsertPayload payload, List<IFormFile>? imageFiles)
        {
            ValidatePayload(payload);

            var quotation = quotationId.HasValue
                ? await _quotationRepository.FindByIdAsync(quotationId.Value)
                : null;

            if (quotationId.HasValue && quotation == null)
            {
                return null;
            }

            var now = DateTime.UtcNow;
            var images = payload.Images != null
                ? payload.Images.Where(i => !string.IsNullOrWhiteSpace(i)).ToList()
                : (quotation == null ? new List<string>() : ParseImageList(quotation.Images));

            if (imageFiles != null && imageFiles.Count > 0)
            {
                var uploaded = await UploadImagesAsync(payload.Title, imageFiles);
                images.AddRange(uploaded);
            }

            var normalizedImages = images.Distinct().ToList();
            var imageJson = JsonSerializer.Serialize(normalizedImages);

            if (quotation == null)
            {
                quotation = new Quotation
                {
                    QuotationId = await _quotationRepository.GetNextIdAsync(),
                    Title = payload.Title,
                    Date = payload.Date ?? now,
                    ClientId = payload.ClientId,
                    Description = payload.Description,
                    Status = payload.Status,
                    ProductType = payload.ProductType,
                    Images = imageJson,
                    LastEditedByName = payload.LastEditedByName
                };

                await _quotationRepository.InsertAsync(quotation);
            }
            else
            {
                quotation.Title = payload.Title;
                quotation.Date = payload.Date ?? quotation.Date;
                quotation.ClientId = payload.ClientId;
                quotation.Description = payload.Description;
                quotation.Status = payload.Status;
                quotation.ProductType = payload.ProductType;
                quotation.Images = imageJson;
                quotation.LastEditedByName = payload.LastEditedByName;

                await _quotationRepository.UpdateAsync(quotation);
            }

            var versionInput = payload.Version ?? new QuotationVersionInput();
            var version = await _versionRepository.FindLatestByQuotationIdAsync(quotation.QuotationId);
            var isNewVersion = version == null;

            if (version == null)
            {
                version = new QuotationVersion
                {
                    QuotationVersionId = await _versionRepository.GetNextIdAsync(),
                    QuotationId = quotation.QuotationId,
                    VersionNumber = versionInput.VersionNumber ?? 1
                };
            }

            version.SubDescription = versionInput.SubDescription;
            if (versionInput.VersionNumber.HasValue && versionInput.VersionNumber.Value > 0)
            {
                version.VersionNumber = versionInput.VersionNumber.Value;
            }
            version.ProfitMargin = versionInput.ProfitMargin;
            version.ExtraCostsJson = JsonSerializer.Serialize(versionInput.ExtraCosts ?? new List<ExtraCostItem>());
            version.ProductTax = versionInput.ProductTax;
            version.LaborCost = versionInput.LaborCost;
            version.LastEditedByName = versionInput.LastEditedByName;
            version.Discount = versionInput.Discount;

            var suppliesInputs = payload.Supplies ?? new List<QuotationSuppliesInput>();
            var assetsInputs = payload.Assets ?? new List<QuotationAssetsInput>();

            var suppliesMap = await GetSuppliesByIdAsync(suppliesInputs.Select(s => s.SuppliesId));
            var assetsMap = await GetAssetsByIdAsync(assetsInputs.Select(a => a.AssetsId));
            var costOfElectricity = await GetCostOfElectricityAsync();

            var suppliesItems = await BuildSuppliesQuotationAsync(suppliesInputs, suppliesMap, version.QuotationVersionId);
            var assetsItems = await BuildAssetsQuotationAsync(assetsInputs, assetsMap, version.QuotationVersionId, costOfElectricity);

            var suppliesTotal = suppliesItems.Sum(i => i.SubTotal);
            var assetsTotal = assetsItems.Sum(i => i.SubTotal);
            var pricing = _pricingService.Calculate(versionInput, suppliesTotal, assetsTotal);

            version.Profit = pricing.Profit;
            version.SubTotal = pricing.SubTotal;
            version.TotalCost = pricing.TotalCost;

            if (version.QuotationVersionId == 0)
            {
                version.QuotationVersionId = await _versionRepository.GetNextIdAsync();
            }

            if (isNewVersion)
            {
                await _versionRepository.InsertAsync(version);
            }
            else
            {
                await _versionRepository.UpdateAsync(version);
            }

            await _suppliesQuotationRepository.DeleteByQuotationVersionIdAsync(version.QuotationVersionId);
            await _assetsQuotationRepository.DeleteByQuotationVersionIdAsync(version.QuotationVersionId);

            if (suppliesItems.Count > 0)
            {
                await _suppliesQuotationRepository.InsertManyAsync(suppliesItems);
            }

            if (assetsItems.Count > 0)
            {
                await _assetsQuotationRepository.InsertManyAsync(assetsItems);
            }

            return await BuildQuotationDetailAsync(quotation.QuotationId, quotation, version, suppliesItems, assetsItems, suppliesMap, assetsMap);
        }

        private static void ValidatePayload(QuotationUpsertPayload payload)
        {
            if (!string.IsNullOrWhiteSpace(payload.Description) && payload.Description.Length > 250)
            {
                throw new InvalidOperationException("Descripcion maxima de 250 caracteres.");
            }
        }

        private async Task<QuotationDetailResponse?> BuildQuotationDetailAsync(int quotationId)
        {
            var quotation = await _quotationRepository.FindByIdAsync(quotationId);
            if (quotation == null)
            {
                return null;
            }

            var version = await _versionRepository.FindLatestByQuotationIdAsync(quotationId);
            if (version == null)
            {
                return new QuotationDetailResponse
                {
                    Quotation = await MapQuotationAsync(quotation),
                    Version = null
                };
            }

            var supplies = await _suppliesQuotationRepository.FindByQuotationVersionIdAsync(version.QuotationVersionId);
            var assets = await _assetsQuotationRepository.FindByQuotationVersionIdAsync(version.QuotationVersionId);

            var suppliesMap = await GetSuppliesByIdAsync(supplies.Select(s => s.SuppliesId));
            var assetsMap = await GetAssetsByIdAsync(assets.Select(a => a.AssetsId));

            return await BuildQuotationDetailAsync(quotationId, quotation, version, supplies, assets, suppliesMap, assetsMap);
        }

        private async Task<QuotationDetailResponse> BuildQuotationDetailAsync(
            int quotationId,
            Quotation quotation,
            QuotationVersion version,
            List<SuppliesQuotation> supplies,
            List<AssetsQuotation> assets,
            Dictionary<string, Supplies> suppliesMap,
            Dictionary<string, Assets> assetsMap)
        {
            var supplyResponses = new List<QuotationSuppliesResponse>();
            foreach (var item in supplies)
            {
                suppliesMap.TryGetValue(item.SuppliesId ?? string.Empty, out var supply);
                supplyResponses.Add(new QuotationSuppliesResponse
                {
                    SuppliesQuotationId = item.SuppliesQuotationId,
                    SuppliesId = item.SuppliesId,
                    UsageQuantity = item.UsageQuantity,
                    Cost = item.Cost,
                    SubTotal = item.SubTotal,
                    LastEditedByName = item.LastEditedByName,
                    Supply = supply == null ? null : await MapSupplyDetailsAsync(supply)
                });
            }

            var assetResponses = new List<QuotationAssetsResponse>();
            foreach (var item in assets)
            {
                assetsMap.TryGetValue(item.AssetsId ?? string.Empty, out var asset);
                assetResponses.Add(new QuotationAssetsResponse
                {
                    AssetsQuotationId = item.AssetsQuotationId,
                    AssetsId = item.AssetsId,
                    UsageQuantity = item.UsageQuantity,
                    Cost = item.Cost,
                    SubTotal = item.SubTotal,
                    Asset = asset == null ? null : await MapAssetDetailsAsync(asset)
                });
            }

            return new QuotationDetailResponse
            {
                Quotation = await MapQuotationAsync(quotation),
                Version = MapVersion(version),
                Supplies = supplyResponses,
                Assets = assetResponses,
                CostBreakdown = BuildCostBreakdown(supplyResponses, supplies, assets, version)
            };
        }

        private async Task<QuotationResponse> MapQuotationAsync(Quotation quotation)
        {
            return new QuotationResponse
            {
                QuotationId = quotation.QuotationId,
                Title = quotation.Title,
                Date = quotation.Date,
                ClientId = quotation.ClientId,
                Description = quotation.Description,
                Status = quotation.Status,
                ProductType = quotation.ProductType,
                Images = await BuildImageUrlsAsync(ParseImageList(quotation.Images)),
                LastEditedByName = quotation.LastEditedByName
            };
        }

        private static QuotationVersionResponse MapVersion(QuotationVersion version)
        {
            return new QuotationVersionResponse
            {
                QuotationVersionId = version.QuotationVersionId,
                QuotationId = version.QuotationId,
                SubDescription = version.SubDescription,
                VersionNumber = version.VersionNumber,
                ProfitMargin = version.ProfitMargin,
                Profit = version.Profit,
                ExtraCosts = ParseExtraCosts(version.ExtraCostsJson),
                ProductTax = version.ProductTax,
                LaborCost = version.LaborCost,
                LastEditedByName = version.LastEditedByName,
                Discount = version.Discount,
                SubTotal = version.SubTotal,
                TotalCost = version.TotalCost
            };
        }

        private static CostBreakdownResponse BuildCostBreakdown(
            List<QuotationSuppliesResponse> supplyResponses,
            List<SuppliesQuotation> supplies,
            List<AssetsQuotation> assets,
            QuotationVersion version)
        {
            var materialsCost = supplies.Sum(s => s.SubTotal);
            var assetsCost = assets.Sum(a => a.SubTotal);
            var extraCosts = ParseExtraCosts(version.ExtraCostsJson).Sum(e => e.Cost);
            var baseCost = materialsCost + assetsCost + version.LaborCost + extraCosts;
            var productTax = version.ProductTax ?? 0m;
            var suppliesBreakdown = supplyResponses.Select(item => new SuppliesCostBreakdown
            {
                SuppliesId = item.SuppliesId,
                Name = item.Supply?.Name,
                UsageQuantity = item.UsageQuantity,
                UnitCost = item.UsageQuantity > 0 ? item.Cost / item.UsageQuantity : 0m,
                Cost = item.Cost,
                SubTotal = item.SubTotal
            }).ToList();

            return new CostBreakdownResponse
            {
                MaterialsCost = materialsCost,
                AssetsCost = assetsCost,
                LaborCost = version.LaborCost,
                ExtraCosts = extraCosts,
                BaseCost = baseCost,
                Supplies = suppliesBreakdown,
                ProfitMargin = version.ProfitMargin,
                Profit = version.Profit,
                SubTotal = version.SubTotal,
                Discount = version.Discount,
                ProductTax = productTax,
                TotalCost = version.TotalCost
            };
        }

        private async Task<QuotationContextResponse> BuildContextAsync()
        {
            var supplies = await SearchSuppliesAsync(null);
            var assets = await SearchAssetsAsync(null);

            return new QuotationContextResponse
            {
                AssetsCatalog = assets,
                SuppliesCatalog = supplies,
                ProductTypes = BuildEnumOptions<ProductType>(),
                QuoteStatuses = BuildEnumOptions<QuoteStatus>(),
                CostOfElectricity = await GetCostOfElectricityAsync()
            };
        }

        private async Task<Dictionary<string, Supplies>> GetSuppliesByIdAsync(IEnumerable<string?> ids)
        {
            var uniqueIds = ids.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();
            var tasks = uniqueIds.Select(id => _suppliesRepository.FindByIdAsync(id!)).ToList();
            var supplies = await Task.WhenAll(tasks);

            return supplies
                .Where(s => s != null && !string.IsNullOrWhiteSpace(s.Id))
                .ToDictionary(s => s!.Id!, s => s!);
        }

        private async Task<Dictionary<string, Assets>> GetAssetsByIdAsync(IEnumerable<string?> ids)
        {
            var uniqueIds = ids.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();
            var tasks = uniqueIds.Select(id => _assetsRepository.FindByIdAsync(id!)).ToList();
            var assets = await Task.WhenAll(tasks);

            return assets
                .Where(a => a != null && !string.IsNullOrWhiteSpace(a.Id))
                .ToDictionary(a => a!.Id!, a => a!);
        }

        private async Task<List<SuppliesQuotation>> BuildSuppliesQuotationAsync(
            List<QuotationSuppliesInput> inputs,
            Dictionary<string, Supplies> suppliesMap,
            int quotationVersionId)
        {
            var results = new List<SuppliesQuotation>();
            var nextId = await _suppliesQuotationRepository.GetNextIdAsync();

            foreach (var input in inputs)
            {
                if (string.IsNullOrWhiteSpace(input.SuppliesId))
                {
                    throw new InvalidOperationException("SuppliesId requerido.");
                }

                if (input.UsageQuantity <= 0)
                {
                    throw new InvalidOperationException("UsageQuantity debe ser mayor a cero.");
                }

                if (!suppliesMap.TryGetValue(input.SuppliesId, out var supply))
                {
                    throw new InvalidOperationException("Insumo no encontrado.");
                }

                var unitCost = ResolveSupplyUnitCost(supply);
                var cost = input.UsageQuantity * unitCost;

                results.Add(new SuppliesQuotation
                {
                    SuppliesQuotationId = nextId++,
                    SuppliesId = input.SuppliesId,
                    UsageQuantity = input.UsageQuantity,
                    Cost = cost,
                    SubTotal = cost,
                    QuotationVersionId = quotationVersionId
                });
            }

            return results;
        }

        private async Task<List<AssetsQuotation>> BuildAssetsQuotationAsync(
            List<QuotationAssetsInput> inputs,
            Dictionary<string, Assets> assetsMap,
            int quotationVersionId,
            decimal costOfElectricity)
        {
            var results = new List<AssetsQuotation>();
            var nextId = await _assetsQuotationRepository.GetNextIdAsync();

            foreach (var input in inputs)
            {
                if (string.IsNullOrWhiteSpace(input.AssetsId))
                {
                    throw new InvalidOperationException("AssetsId requerido.");
                }

                if (input.UsageQuantity <= 0)
                {
                    throw new InvalidOperationException("UsageQuantity debe ser mayor a cero.");
                }

                if (!assetsMap.TryGetValue(input.AssetsId, out var asset))
                {
                    throw new InvalidOperationException("Activo no encontrado.");
                }

                var cost = input.UsageQuantity * asset.Electricity * costOfElectricity;

                results.Add(new AssetsQuotation
                {
                    AssetsQuotationId = nextId++,
                    AssetsId = input.AssetsId,
                    UsageQuantity = input.UsageQuantity,
                    Cost = cost,
                    SubTotal = cost,
                    QuotationVersionId = quotationVersionId
                });
            }

            return results;
        }

        private async Task<SuppliesResponse> MapSupplyToResponseAsync(Supplies supply)
        {
            return new SuppliesResponse
            {
                Id = supply.Id,
                Type = supply.Type,
                Name = supply.Name,
                Color = supply.Color,
                Brand = supply.Brand,
                Image = await BuildFileUrlAsync(supply.Image),
                LastCost = supply.LastCost,
                CostQuantity = supply.CostQuantity,
                UnitCost = supply.UnitCost,
                Description = supply.Description,
                IsActive = supply.IsActive,
                Tax = supply.Tax,
                UnitsMeasurementId = supply.UnitsMeasurementId
            };
        }

        private async Task<AssetsResponse> MapAssetToResponseAsync(Assets asset)
        {
            return new AssetsResponse
            {
                Id = asset.Id,
                Name = asset.Name,
                Description = asset.Description,
                Price = asset.Price,
                Image = await BuildFileUrlAsync(asset.Image),
                Electricity = asset.Electricity,
                WearType = asset.WearType
            };
        }

        private async Task<SupplyDetailsResponse> MapSupplyDetailsAsync(Supplies supply)
        {
            return new SupplyDetailsResponse
            {
                Id = supply.Id,
                Type = supply.Type,
                Name = supply.Name,
                Color = supply.Color,
                Brand = supply.Brand,
                Image = await BuildFileUrlAsync(supply.Image),
                LastCost = supply.LastCost,
                CostQuantity = supply.CostQuantity,
                UnitCost = supply.UnitCost,
                Description = supply.Description,
                IsActive = supply.IsActive,
                Tax = supply.Tax,
                UnitsMeasurementId = supply.UnitsMeasurementId
            };
        }

        private static decimal ResolveSupplyUnitCost(Supplies supply)
        {
            if (supply.UnitCost.HasValue && supply.UnitCost.Value > 0)
            {
                return supply.UnitCost.Value;
            }

            if (supply.LastCost.HasValue
                && supply.CostQuantity.HasValue
                && supply.CostQuantity.Value > 0)
            {
                return supply.LastCost.Value / supply.CostQuantity.Value;
            }

            throw new InvalidOperationException("Insumo sin costo unitario valido.");
        }

        private async Task<AssetDetailsResponse> MapAssetDetailsAsync(Assets asset)
        {
            return new AssetDetailsResponse
            {
                Id = asset.Id,
                Name = asset.Name,
                Description = asset.Description,
                Price = asset.Price,
                Image = await BuildFileUrlAsync(asset.Image),
                Electricity = asset.Electricity,
                WearType = asset.WearType
            };
        }

        private async Task<List<string>> UploadImagesAsync(string? title, IEnumerable<IFormFile> files)
        {
            var uploaded = new List<string>();
            var safeTitle = string.IsNullOrWhiteSpace(title) ? "quotation" : title.Trim().Replace(" ", "-");
            var dateStamp = DateTime.UtcNow.ToString("yyyyMMdd");

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                {
                    continue;
                }

                if (file.ContentType == null || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Solo se permiten imagenes.");
                }

                var key = $"images/quotations/{safeTitle}-{dateStamp}-{Guid.NewGuid()}-{file.FileName}";
                await using var stream = file.OpenReadStream();

                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketSettings.BucketName,
                    Key = key,
                    InputStream = stream,
                    ContentType = file.ContentType
                };

                await _s3Client.PutObjectAsync(putRequest);
                uploaded.Add(key);
            }

            return uploaded;
        }

        private async Task<List<string>> BuildImageUrlsAsync(List<string> keys)
        {
            var results = new List<string>();
            foreach (var key in keys)
            {
                var url = await BuildFileUrlAsync(key);
                if (!string.IsNullOrWhiteSpace(url))
                {
                    results.Add(url);
                }
            }

            return results;
        }

        private async Task<string?> BuildFileUrlAsync(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            if (key.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || key.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return key;
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

        private async Task<decimal> GetCostOfElectricityAsync()
        {
            var config = await _systemConfigurations.Find(_ => true).FirstOrDefaultAsync();
            if (config == null || !config.CostOfElectricity.HasValue)
            {
                return 0m;
            }

            return (decimal)config.CostOfElectricity.Value;
        }

        private static List<EnumOptionResponse> BuildEnumOptions<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<Enum>()
                .Select(value => new EnumOptionResponse
                {
                    Value = Convert.ToInt32(value),
                    Name = value.ToString()
                })
                .ToList();
        }

        private static List<string> ParseImageList(string? imagesJson)
        {
            if (string.IsNullOrWhiteSpace(imagesJson))
            {
                return new List<string>();
            }

            try
            {
                var list = JsonSerializer.Deserialize<List<string>>(imagesJson);
                if (list != null)
                {
                    return list.Where(i => !string.IsNullOrWhiteSpace(i)).ToList();
                }
            }
            catch (JsonException)
            {
                // Fallback to raw string
            }

            return new List<string> { imagesJson };
        }

        private static List<ExtraCostItem> ParseExtraCosts(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<ExtraCostItem>();
            }

            try
            {
                var list = JsonSerializer.Deserialize<List<ExtraCostItem>>(json);
                return list ?? new List<ExtraCostItem>();
            }
            catch (JsonException)
            {
                return new List<ExtraCostItem>();
            }
        }
    }
}
