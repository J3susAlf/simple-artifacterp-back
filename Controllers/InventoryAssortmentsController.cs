using Microsoft.AspNetCore.Mvc;
using simple_artifacterp_back.DTOs;
using simple_artifacterp_back.Enums;
using simple_artifacterp_back.Services;

namespace simple_artifacterp_back.Controllers
{
    [ApiController]
    [Route("api/inventory/assortments")]
    public class InventoryAssortmentsController : ControllerBase
    {
        private readonly InventoryAssortmentService _assortmentService;

        public InventoryAssortmentsController(InventoryAssortmentService assortmentService)
        {
            _assortmentService = assortmentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAssortment([FromBody] AssortmentCreateRequest request)
        {
            try
            {
                var result = await _assortmentService.CreateAsync(request);
                return result == null ? NotFound("Insumo no encontrado o id invalido.") : Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/finalize")]
        public async Task<IActionResult> FinalizeAssortment(int id, [FromBody] FinalizeAssortmentRequest? request)
        {
            var result = await _assortmentService.FinalizeAsync(id, request);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost("direct-inventory")]
        public async Task<IActionResult> RegisterDirectInventory([FromBody] DirectInventoryRequest request)
        {
            try
            {
                var result = await _assortmentService.RegisterDirectInventoryAsync(request);
                return result == null ? NotFound("Insumo no encontrado o id invalido.") : Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAssortments(
            [FromQuery] int? year,
            [FromQuery] int? month,
            [FromQuery] AssortmentStatus? status,
            [FromQuery] SupplyType? supplyType)
        {
            if (month.HasValue ^ year.HasValue)
            {
                return BadRequest("Debe enviar mes y anio para filtrar por fecha.");
            }

            if (month.HasValue && (month.Value < 1 || month.Value > 12))
            {
                return BadRequest("Mes invalido.");
            }

            var result = await _assortmentService.GetAssortmentsAsync(year, month, status, supplyType);
            return Ok(result);
        }

        [HttpGet("inventory-supplies")]
        public async Task<IActionResult> GetInventorySupplies()
        {
            var result = await _assortmentService.GetInventorySuppliesAsync();
            return Ok(result);
        }
    }
}
