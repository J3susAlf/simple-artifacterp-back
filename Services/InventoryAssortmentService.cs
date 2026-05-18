using simple_artifacterp_back.DTOs;
using simple_artifacterp_back.Enums;
using simple_artifacterp_back.Models;
using simple_artifacterp_back.Repositories;

namespace simple_artifacterp_back.Services
{
    public class InventoryAssortmentService
    {
        private readonly AssortmentRepository _assortmentRepository;
        private readonly SuppliesRepository _suppliesRepository;
        private readonly InventorySuppliesRepository _inventoryRepository;

        public InventoryAssortmentService(
            AssortmentRepository assortmentRepository,
            SuppliesRepository suppliesRepository,
            InventorySuppliesRepository inventoryRepository)
        {
            _assortmentRepository = assortmentRepository;
            _suppliesRepository = suppliesRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<AssortmentResponse?> CreateAsync(AssortmentCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.SuppliesId))
            {
                return null;
            }

            var supply = await _suppliesRepository.FindByIdAsync(request.SuppliesId);
            if (supply == null)
            {
                return null;
            }

            if (request.PurchaseQuantity <= 0 || request.UnitaryPurchaseCost <= 0)
            {
                throw new InvalidOperationException("Cantidad y costo unitario deben ser mayores a cero.");
            }

            if (request.IsPack && request.PackQuantity <= 0)
            {
                throw new InvalidOperationException("PackQuantity debe ser mayor a cero cuando es pack.");
            }

            var packQuantity = request.IsPack ? request.PackQuantity : 1;
            var status = request.ImmediateDelivery == true ? AssortmentStatus.delivered : AssortmentStatus.Pending;
            var deliveryDate = request.ImmediateDelivery == true
                ? DateTime.UtcNow
                : request.DeliveryDate ?? DateTime.UtcNow.Date.AddDays(1);

            var assortment = new Assortment
            {
                AssortmentId = await _assortmentRepository.GetNextIdAsync(),
                UnitaryPurchaseCost = request.UnitaryPurchaseCost,
                IsPack = request.IsPack,
                PackQuantity = packQuantity,
                PurchaseQuantity = request.PurchaseQuantity,
                SuppliesId = request.SuppliesId,
                LastEditedByName = request.LastEditedByName,
                Status = status,
                DeliveryDate = deliveryDate
            };

            await _assortmentRepository.CreateAsync(assortment);

            if (request.ImmediateDelivery == true)
            {
                await ApplyAssortmentToInventoryAsync(assortment, supply);
            }

            return MapToResponse(assortment);
        }

        public async Task<AssortmentResponse?> FinalizeAsync(int assortmentId, FinalizeAssortmentRequest? request)
        {
            var assortment = await _assortmentRepository.GetByIdAsync(assortmentId);
            if (assortment == null || string.IsNullOrWhiteSpace(assortment.SuppliesId))
            {
                return null;
            }

            if (request?.LastEditedByName != null)
            {
                assortment.LastEditedByName = request.LastEditedByName;
            }

            if (assortment.Status == AssortmentStatus.delivered)
            {
                return MapToResponse(assortment);
            }

            var supply = await _suppliesRepository.FindByIdAsync(assortment.SuppliesId);
            if (supply == null)
            {
                return null;
            }

            assortment.Status = AssortmentStatus.delivered;
            assortment.DeliveryDate = DateTime.UtcNow;

            await _assortmentRepository.UpdateAsync(assortment.AssortmentId, assortment);
            await ApplyAssortmentToInventoryAsync(assortment, supply);

            return MapToResponse(assortment);
        }

        public async Task<InventorySuppliesResponse?> RegisterDirectInventoryAsync(DirectInventoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.SuppliesId))
            {
                return null;
            }

            var supply = await _suppliesRepository.FindByIdAsync(request.SuppliesId);
            if (supply == null)
            {
                return null;
            }

            if (request.AvailableQuantity <= 0)
            {
                throw new InvalidOperationException("AvailableQuantity debe ser mayor a cero.");
            }

            var inventory = await _inventoryRepository.FindBySuppliesIdAsync(request.SuppliesId);
            if (inventory == null)
            {
                inventory = new InventorySupplies
                {
                    InventorySuppliesId = await _inventoryRepository.GetNextIdAsync(),
                    SuppliesId = request.SuppliesId,
                    UnitsMeasurementId = request.UnitsMeasurementId ?? supply.UnitsMeasurementId,
                    AvailableQuantity = request.AvailableQuantity,
                    MinimumQuantity = request.MinimumQuantity,
                    CommittedQuantity = request.CommittedQuantity,
                    LastDispatchNumber = "DirectAssortment"
                };

                await _inventoryRepository.InsertAsync(inventory);
            }
            else
            {
                inventory.AvailableQuantity += request.AvailableQuantity;
                inventory.MinimumQuantity = request.MinimumQuantity;
                inventory.CommittedQuantity = request.CommittedQuantity;
                inventory.UnitsMeasurementId = request.UnitsMeasurementId ?? inventory.UnitsMeasurementId ?? supply.UnitsMeasurementId;
                inventory.LastDispatchNumber = "DirectAssortment";

                await _inventoryRepository.UpdateAsync(inventory);
            }

            supply.LastCost = request.LastCost;
            if (inventory.AvailableQuantity > 0)
            {
                supply.IsActive = true;
            }

            await _suppliesRepository.UpdateAsync(supply);

            return MapToResponse(inventory);
        }

        public async Task<List<AssortmentWithSupplyResponse>> GetAssortmentsAsync(
            int? year,
            int? month,
            AssortmentStatus? status,
            SupplyType? supplyType)
        {
            var assortments = await _assortmentRepository.GetAllAsync();
            var effectiveStatus = status ?? AssortmentStatus.Pending;

            var filtered = assortments
                .Where(a => a.Status == effectiveStatus)
                .ToList();

            if (year.HasValue && month.HasValue)
            {
                filtered = filtered
                    .Where(a => a.DeliveryDate.HasValue
                        && a.DeliveryDate.Value.Year == year.Value
                        && a.DeliveryDate.Value.Month == month.Value)
                    .ToList();
            }

            var supplyIds = filtered
                .Select(a => a.SuppliesId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .ToList();

            var supplyTasks = supplyIds
                .Select(id => _suppliesRepository.FindByIdAsync(id!))
                .ToList();

            var supplies = await Task.WhenAll(supplyTasks);
            var supplyById = supplies
                .Where(s => s != null)
                .ToDictionary(s => s!.Id!, s => s!);

            var results = new List<AssortmentWithSupplyResponse>();
            foreach (var assortment in filtered)
            {
                if (string.IsNullOrWhiteSpace(assortment.SuppliesId)
                    || !supplyById.TryGetValue(assortment.SuppliesId, out var supply))
                {
                    continue;
                }

                if (supplyType.HasValue && supply.Type != supplyType.Value)
                {
                    continue;
                }

                results.Add(MapToResponse(assortment, supply));
            }

            return results;
        }

        public async Task<List<InventorySuppliesWithSupplyResponse>> GetInventorySuppliesAsync()
        {
            var inventoryList = await _inventoryRepository.FindAllAsync();
            var supplyIds = inventoryList
                .Select(i => i.SuppliesId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .ToList();

            var supplyTasks = supplyIds
                .Select(id => _suppliesRepository.FindByIdAsync(id!))
                .ToList();

            var supplies = await Task.WhenAll(supplyTasks);
            var supplyById = supplies
                .Where(s => s != null)
                .ToDictionary(s => s!.Id!, s => s!);

            var results = new List<InventorySuppliesWithSupplyResponse>();
            foreach (var inventory in inventoryList)
            {
                supplyById.TryGetValue(inventory.SuppliesId ?? string.Empty, out var supply);

                results.Add(new InventorySuppliesWithSupplyResponse
                {
                    InventorySuppliesId = inventory.InventorySuppliesId,
                    AvailableQuantity = inventory.AvailableQuantity,
                    MinimumQuantity = inventory.MinimumQuantity,
                    CommittedQuantity = inventory.CommittedQuantity,
                    UnitsMeasurementId = inventory.UnitsMeasurementId,
                    SuppliesId = inventory.SuppliesId,
                    LastDispatchNumber = inventory.LastDispatchNumber,
                    Supply = supply == null ? null : new SupplyDetailsResponse
                    {
                        Id = supply.Id,
                        Type = supply.Type,
                        Name = supply.Name,
                        Color = supply.Color,
                        Brand = supply.Brand,
                        Image = supply.Image,
                        LastCost = supply.LastCost,
                        Description = supply.Description,
                        IsActive = supply.IsActive,
                        Tax = supply.Tax,
                        UnitsMeasurementId = supply.UnitsMeasurementId
                    }
                });
            }

            return results;
        }

        private async Task ApplyAssortmentToInventoryAsync(Assortment assortment, Supplies supply)
        {
            var packQuantity = assortment.IsPack ? Math.Max(1, assortment.PackQuantity) : 1;
            var incomingQuantity = packQuantity * assortment.PurchaseQuantity;
            var unitCost = assortment.IsPack
                ? assortment.UnitaryPurchaseCost / packQuantity
                : assortment.UnitaryPurchaseCost;

            var inventory = await _inventoryRepository.FindBySuppliesIdAsync(assortment.SuppliesId ?? string.Empty);
            if (inventory == null)
            {
                inventory = new InventorySupplies
                {
                    InventorySuppliesId = await _inventoryRepository.GetNextIdAsync(),
                    SuppliesId = assortment.SuppliesId,
                    UnitsMeasurementId = supply.UnitsMeasurementId,
                    AvailableQuantity = incomingQuantity,
                    MinimumQuantity = 0,
                    CommittedQuantity = 0,
                    LastDispatchNumber = BuildDispatchNumber(assortment)
                };

                await _inventoryRepository.InsertAsync(inventory);
            }
            else
            {
                inventory.AvailableQuantity += incomingQuantity;
                inventory.LastDispatchNumber = BuildDispatchNumber(assortment);
                if (string.IsNullOrWhiteSpace(inventory.UnitsMeasurementId))
                {
                    inventory.UnitsMeasurementId = supply.UnitsMeasurementId;
                }

                await _inventoryRepository.UpdateAsync(inventory);
            }

            supply.LastCost = unitCost;
            if (inventory.AvailableQuantity > 0)
            {
                supply.IsActive = true;
            }

            await _suppliesRepository.UpdateAsync(supply);
        }

        private static string BuildDispatchNumber(Assortment assortment)
        {
            return $"{assortment.AssortmentId}-{assortment.UnitaryPurchaseCost}-{assortment.LastEditedByName}-{assortment.SuppliesId}";
        }

        private static AssortmentResponse MapToResponse(Assortment assortment)
        {
            return new AssortmentResponse
            {
                AssortmentId = assortment.AssortmentId,
                SuppliesId = assortment.SuppliesId,
                UnitaryPurchaseCost = assortment.UnitaryPurchaseCost,
                IsPack = assortment.IsPack,
                PackQuantity = assortment.PackQuantity,
                PurchaseQuantity = assortment.PurchaseQuantity,
                LastEditedByName = assortment.LastEditedByName,
                Status = assortment.Status,
                DeliveryDate = assortment.DeliveryDate
            };
        }

        private static InventorySuppliesResponse MapToResponse(InventorySupplies inventory)
        {
            return new InventorySuppliesResponse
            {
                InventorySuppliesId = inventory.InventorySuppliesId,
                AvailableQuantity = inventory.AvailableQuantity,
                MinimumQuantity = inventory.MinimumQuantity,
                CommittedQuantity = inventory.CommittedQuantity,
                UnitsMeasurementId = inventory.UnitsMeasurementId,
                SuppliesId = inventory.SuppliesId,
                LastDispatchNumber = inventory.LastDispatchNumber
            };
        }

        private static AssortmentWithSupplyResponse MapToResponse(Assortment assortment, Supplies supply)
        {
            return new AssortmentWithSupplyResponse
            {
                AssortmentId = assortment.AssortmentId,
                SuppliesId = assortment.SuppliesId,
                UnitaryPurchaseCost = assortment.UnitaryPurchaseCost,
                IsPack = assortment.IsPack,
                PackQuantity = assortment.PackQuantity,
                PurchaseQuantity = assortment.PurchaseQuantity,
                LastEditedByName = assortment.LastEditedByName,
                Status = assortment.Status,
                DeliveryDate = assortment.DeliveryDate,
                Supply = new SupplyDetailsResponse
                {
                    Id = supply.Id,
                    Type = supply.Type,
                    Name = supply.Name,
                    Color = supply.Color,
                    Brand = supply.Brand,
                    Image = supply.Image,
                    LastCost = supply.LastCost,
                    Description = supply.Description,
                    IsActive = supply.IsActive,
                    Tax = supply.Tax,
                    UnitsMeasurementId = supply.UnitsMeasurementId
                }
            };
        }
    }
}
