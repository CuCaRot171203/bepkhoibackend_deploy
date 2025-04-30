using BepKhoiBackend.BusinessObject.dtos.RoomDto;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace BepKhoiBackend.BusinessObject.Services.RoomService
{
    public interface IRoomService
    {
        Task<FileDataDto> DownloadQRCodeAsync(int roomId);

        Task DeleteQRCodeAsync(int roomId);
        Task UpdateQRCodeUrlAsync(int roomId, string qrCodeUrl);
        Task<string> GenerateQRCodeAndSaveAsync(int roomId, String UrlBase);
        Task<IEnumerable<RoomDto>> GetAllAsync(int limit, int offset);
        Task<RoomDto> GetByIdAsync(int id);
        Task AddAsync(RoomCreateDto roomCreateDto);
        Task UpdateAsync(int id, RoomUpdateDto roomUpdateDto);
        Task<bool> SoftDeleteAsync(int id);
        Task<IEnumerable<RoomDto>> SearchByNameAsync(string roomName, int limit, int offset);    
        // Service - Thanh Tung
        Task<List<RoomDtoPos>> GetRoomAsyncForPos();
        Task<List<RoomDtoPos>> FilterRoomAsyncPos(int? roomAreaId, bool? isUse);
        //Task<List<RoomDtoPos>> SearchRoomPosAsync(string searchString);
        Task<bool> UpdateRoomNoteAsync(RoomNoteUpdateDto dto);
    }
}
