using BepKhoiBackend.BusinessObject.dtos.RoomAreaDto;
using BepKhoiBackend.BusinessObject.Services.RoomAreaService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BepKhoiBackend.API.Controllers.RoomAreaControllers
{
    [Route("api/roomarea")]
    [ApiController]
    public class RoomAreaController : ControllerBase
    {
        private readonly IRoomAreaService _roomAreaService;

        public RoomAreaController(IRoomAreaService roomAreaService)
        {
            _roomAreaService = roomAreaService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll([FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            try
            {
                var result = await _roomAreaService.GetAllAsync(limit, offset);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetByIdAndIsDelete(
        [FromQuery] bool isDelete,
        [FromQuery] int limit = 10,
        [FromQuery] int offset = 0)
        {
            try
            {
                var result = await _roomAreaService.GetByIdAndIsDeleteAsync(isDelete, limit, offset);
                if (result == null || !result.Any())
                    return NotFound(new { message = "No RoomArea found with the given criteria" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }


        [HttpGet("search-by-name-id")]
        public async Task<IActionResult> SearchByNameOrId(
        [FromQuery] string name = null,
        [FromQuery] int limit = 10,
        [FromQuery] int offset = 0)
        {
            try
            {
                var result = await _roomAreaService.SearchByNameOrIdAsync(name, limit, offset);

                if (result == null || !result.Any())
                    return NotFound(new { message = "No RoomArea found with the given name or id" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var roomArea = await _roomAreaService.GetByIdAsync(id);
                if (roomArea == null)
                    return NotFound(new { message = "RoomArea not found" });

                return Ok(roomArea);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [Authorize(Roles = "manager")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] RoomAreaDto roomAreaDto)
        {
            try
            {
                if (roomAreaDto == null)
                    return BadRequest(new { message = "Invalid data" });

                await _roomAreaService.AddAsync(roomAreaDto);
                return Ok(new { message = "RoomArea created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [Authorize(Roles = "manager")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RoomAreaDto roomAreaDto)
        {
            try
            {
                if (roomAreaDto == null)
                    return BadRequest(new { message = "Invalid data" });

                roomAreaDto.RoomAreaId = id;
                await _roomAreaService.UpdateAsync(roomAreaDto);
                return Ok(new { message = "RoomArea updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [Authorize(Roles = "manager")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var success = await _roomAreaService.SoftDeleteAsync(id);
                if (!success)
                    return BadRequest(new { message = "Cannot delete RoomArea with existing invoices" });

                return Ok(new { message = "RoomArea deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
