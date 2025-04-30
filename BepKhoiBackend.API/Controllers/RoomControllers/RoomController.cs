using BepKhoiBackend.BusinessObject.dtos.RoomDto;
using BepKhoiBackend.BusinessObject.Services.RoomService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BepKhoiBackend.API.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly QRCodeService _qrCodeService;
        public RoomController(IRoomService roomService, QRCodeService qrCodeService)
        {
            _roomService = roomService;
            _qrCodeService = qrCodeService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll([FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            var result = await _roomService.GetAllAsync(limit, offset);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var room = await _roomService.GetByIdAsync(id);
            if (room == null)
                return NotFound(new { message = "Room not found" });

            return Ok(room);
        }

        [Authorize(Roles = "manager")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] RoomCreateDto roomCreateDto)
        {
            if (roomCreateDto == null)
                return BadRequest(new { message = "Invalid data" });
            await _roomService.AddAsync(roomCreateDto);
            return Ok(new { message = "Room created successfully" });
        }

        [Authorize(Roles = "manager")]
        [HttpPost("generate-qr/{id}")]
        public async Task<IActionResult> GenerateQRCodeForRoom(int id, String UrlBase)
        {
            try
            {
                string qrCodeUrl = await _roomService.GenerateQRCodeAndSaveAsync(id,UrlBase);
                return Ok(new { message = "QR Code generated successfully", qrCodeUrl });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        [Authorize(Roles = "manager")]
        [HttpDelete("delete-qr/{id}")]
        public async Task<IActionResult> DeleteQRCode(int id)
        {
            try
            {
                await _roomService.DeleteQRCodeAsync(id);
                return Ok(new { message = "QR Code deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "manager, cashier")]
        [HttpGet("download-qr/{id}")]
        public async Task<IActionResult> DownloadQRCode(int id)
        {
            try
            {
                var fileData = await _roomService.DownloadQRCodeAsync(id);
                return File(fileData.Content, fileData.ContentType, fileData.FileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "manager")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RoomUpdateDto roomUpdateDto)
        {
            if (roomUpdateDto == null)
                return BadRequest(new { message = "Invalid data" });

            await _roomService.UpdateAsync(id, roomUpdateDto);
            return Ok(new { message = "Room updated successfully" });
        }

        [Authorize(Roles = "manager")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var success = await _roomService.SoftDeleteAsync(id);
            if (!success)
                return BadRequest(new { message = "Room not found or already deleted" });

            return Ok(new { message = "Room deleted successfully" });
        }

        [HttpGet("search-by-name")]
        public async Task<IActionResult> SearchByName([FromQuery] string? name, [FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            var result = await _roomService.SearchByNameAsync(name, limit, offset);
            return Ok(result);
        }

        //controller for get Room for POS site
        [Authorize(Roles = "manager, cashier")]
        [HttpGet("get-all-room-for-pos")]
        [ProducesResponseType(typeof(List<RoomDtoPos>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> TestGetRooms()
        {
            try
            {
                var result = await _roomService.GetRoomAsyncForPos();
                if (result == null || !result.Any())
                {
                    return NotFound(new { message = "Không tìm thấy dữ liệu phòng." });
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "error server.", error = ex.Message });
            }
        }

        [Authorize(Roles = "manager, cashier")]
        [HttpGet("filter-room-pos")]
        [ProducesResponseType(typeof(List<RoomDtoPos>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> FilterRoomPos([FromQuery] int? roomAreaId, [FromQuery] bool? isUse)
        {
            try
            {
                var result = await _roomService.FilterRoomAsyncPos(roomAreaId, isUse);
                if (result == null || !result.Any())
                {
                    return NotFound(new { message = "Can't find data of room." });
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error server.", error = ex.Message });
            }
        }

        [Authorize(Roles = "manager, cashier")]
        [HttpPut("update-room-note")]
        public async Task<IActionResult> UpdateRoomNote([FromBody] RoomNoteUpdateDto dto)
        {
            try
            {
                // Kiểm tra nếu DTO không hợp lệ
                if (dto == null)
                {
                    return BadRequest(new { success = false, message = "Request body cannot be null." });
                }

                // Gọi service để xử lý logic cập nhật ghi chú phòng
                bool isUpdated = await _roomService.UpdateRoomNoteAsync(dto);

                // Nếu không thành công (isUpdated là false), trả về thông báo lỗi
                if (!isUpdated)
                {
                    return StatusCode(500, new { success = false, message = "Update room note failed." });
                }

                // Nếu cập nhật thành công, trả về kết quả
                return Ok(new { success = true, message = "Room note updated successfully." });
            }
            catch (ArgumentNullException ex)
            {
                // Xử lý lỗi nếu DTO không hợp lệ
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Xử lý lỗi khi phòng note rỗng
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                // Xử lý khi phòng không tìm thấy (đã được ném từ repo hoặc service)
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Xử lý lỗi khi có vấn đề với cơ sở dữ liệu hoặc cập nhật không thành công
                return StatusCode(500, new { success = false, message = "Database error occurred while updating the room note.", details = ex.Message });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi không xác định
                return StatusCode(500, new { success = false, message = "An unexpected error occurred while updating the room note.", details = ex.Message });
            }
        }


    }
}
