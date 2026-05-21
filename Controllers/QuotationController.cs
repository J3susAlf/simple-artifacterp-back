using Microsoft.AspNetCore.Mvc;
using simple_artifacterp_back.DTOs;
using simple_artifacterp_back.Enums;
using simple_artifacterp_back.Services;
using System.Text.Json;

namespace simple_artifacterp_back.Controllers
{
    [ApiController]
    [Route("api/quotations")]
    public class QuotationController : ControllerBase
    {
        private readonly QuotationService _quotationService;

        public QuotationController(QuotationService quotationService)
        {
            _quotationService = quotationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuotations(
            [FromQuery] int? year,
            [FromQuery] int? month,
            [FromQuery] ProductType? productType,
            [FromQuery] QuoteStatus? status,
            [FromQuery] string? search)
        {
            if (month.HasValue ^ year.HasValue)
            {
                return BadRequest("Debe enviar mes y anio para filtrar por fecha.");
            }

            if (month.HasValue && (month.Value < 1 || month.Value > 12))
            {
                return BadRequest("Mes invalido.");
            }

            var result = await _quotationService.GetListAsync(year, month, productType, status, search);
            return Ok(result);
        }

        [HttpGet("context")]
        public async Task<IActionResult> GetQuotationContext([FromQuery] int? quotationId)
        {
            var result = await _quotationService.GetContextAsync(quotationId);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateQuotation([FromForm] QuotationUpsertRequest request)
        {
            if (!TryParsePayload(request.PayloadJson, out var payload, out var error))
            {
                return BadRequest(error);
            }

            try
            {
                var result = await _quotationService.CreateAsync(payload!, request.ImageFiles);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateQuotation(int id, [FromForm] QuotationUpsertRequest request)
        {
            if (!TryParsePayload(request.PayloadJson, out var payload, out var error))
            {
                return BadRequest(error);
            }

            try
            {
                var result = await _quotationService.UpdateAsync(id, payload!, request.ImageFiles);
                return result == null ? NotFound() : Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("catalogs/supplies")]
        public async Task<IActionResult> SearchSupplies([FromQuery] string? name)
        {
            var result = await _quotationService.SearchSuppliesAsync(name);
            return Ok(result);
        }

        [HttpGet("catalogs/assets")]
        public async Task<IActionResult> SearchAssets([FromQuery] string? name)
        {
            var result = await _quotationService.SearchAssetsAsync(name);
            return Ok(result);
        }

        private static bool TryParsePayload(string? json, out QuotationUpsertPayload? payload, out string? error)
        {
            payload = null;
            error = null;

            if (string.IsNullOrWhiteSpace(json))
            {
                error = "PayloadJson requerido.";
                return false;
            }

            try
            {
                payload = JsonSerializer.Deserialize<QuotationUpsertPayload>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (payload == null)
                {
                    error = "PayloadJson invalido.";
                    return false;
                }

                return true;
            }
            catch (JsonException)
            {
                error = "PayloadJson invalido.";
                return false;
            }
        }
    }
}
