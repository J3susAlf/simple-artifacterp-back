using simple_artifacterp_back.DTOs;
using simple_artifacterp_back.Models;
using simple_artifacterp_back.Repositories;

namespace simple_artifacterp_back.Services
{
    public class UnitsMeasurementCatalogService
    {
        private readonly UnitsMeasurementRepository _repository;

        public UnitsMeasurementCatalogService(UnitsMeasurementRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<UnitsMeasurementResponse>> GetAllAsync()
        {
            var units = await _repository.FindAllAsync();
            return units.Select(MapToResponse).ToList();
        }

        public async Task<UnitsMeasurementResponse?> GetByIdAsync(string id)
        {
            var unit = await _repository.FindByIdAsync(id);
            return unit == null ? null : MapToResponse(unit);
        }

        public async Task<UnitsMeasurementResponse> CreateAsync(UnitsMeasurementRequest request)
        {
            var unit = new UnitsMeasurement
            {
                Name = request.Name,
                Symbol = request.Symbol,
                Type = request.Type
            };

            await _repository.InsertAsync(unit);
            return MapToResponse(unit);
        }

        public async Task<UnitsMeasurementResponse?> UpdateAsync(string id, UnitsMeasurementRequest request)
        {
            var unit = await _repository.FindByIdAsync(id);
            if (unit == null)
            {
                return null;
            }

            unit.Name = request.Name;
            unit.Symbol = request.Symbol;
            unit.Type = request.Type;

            await _repository.UpdateAsync(unit);
            return MapToResponse(unit);
        }

        public async Task<bool> DeleteAsync(string id, bool forceDelete)
        {
            var unit = await _repository.FindByIdAsync(id);
            if (unit == null)
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

        private static UnitsMeasurementResponse MapToResponse(UnitsMeasurement unit)
        {
            return new UnitsMeasurementResponse
            {
                Id = unit.Id,
                Name = unit.Name,
                Symbol = unit.Symbol,
                Type = unit.Type
            };
        }
    }
}
