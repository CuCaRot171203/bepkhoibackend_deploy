using BepKhoiBackend.BusinessObject.Abstract.UnitAbstract;
using Microsoft.AspNetCore.Mvc;

namespace BepKhoiBackend.API.Controllers.UnitControllers
{
    [Route("api/units")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IUnitService _unitService;

        public UnitController(IUnitService unitService)
        {
            _unitService = unitService;
        }

        [HttpGet("get-all-units")]
        public async Task<IActionResult> GetAllUnits()
        {
            try
            {
                var units = await _unitService.GetUnitsAsync();
                return Ok(units);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
