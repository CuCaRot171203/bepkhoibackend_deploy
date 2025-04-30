using BepKhoiBackend.BusinessObject.dtos.OrderDetailDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDto;
using BepKhoiBackend.BusinessObject.dtos.RoomDto;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository.RoomRepository;
using BepKhoiBackend.DataAccess.Repository.RoomRepository.Interface;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Math;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Services.RoomService
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly QRCodeService _qrCodeService;

        public RoomService(IRoomRepository roomRepository, QRCodeService qrCodeService)
        {
            _roomRepository = roomRepository;
            _qrCodeService = qrCodeService;
        }

        public async Task<IEnumerable<RoomDto>> GetAllAsync(int limit, int offset)
        {
            var rooms = await _roomRepository.GetAllAsync(limit, offset);
            return rooms.Select(r => new RoomDto
            {
                RoomId = r.RoomId,
                RoomName = r.RoomName,
                RoomAreaId = r.RoomAreaId,
                OrdinalNumber = r.OrdinalNumber,
                SeatNumber = r.SeatNumber,
                RoomNote = r.RoomNote,
                QrCodeUrl = r.QrCodeUrl,
                Status = r.Status,
                IsUse = r.IsUse,
                IsDelete = r.IsDelete ?? false
            }).ToList();
        }

        public async Task<RoomDto> GetByIdAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return null;

            return new RoomDto
            {
                RoomId = room.RoomId,
                RoomName = room.RoomName,
                RoomAreaId = room.RoomAreaId,
                OrdinalNumber = room.OrdinalNumber,
                SeatNumber = room.SeatNumber,
                RoomNote = room.RoomNote,
                QrCodeUrl = room.QrCodeUrl,
                Status = room.Status,
                IsUse = room.IsUse,
                IsDelete = room.IsDelete ?? false
            };
        }

        public async Task<IEnumerable<RoomDto>> SearchByIdOrNameAsync(string keyword)
        {
            var rooms = await _roomRepository.SearchByIdOrNameAsync(keyword);
            return rooms.Select(r => new RoomDto
            {
                RoomId = r.RoomId,
                RoomName = r.RoomName
            }).ToList();
        }

        public async Task AddAsync(RoomCreateDto roomCreateDto)
        {
            //string savePath = Path.GetTempPath();
            //string url = $"https://facebook.com/{roomCreateDto.RoomName}&{roomCreateDto.RoomAreaId}";
            //string qrUrl = await _qrCodeService.GenerateAndUploadQRCodeAsync(url, savePath);
            var room = new Room
            {
                RoomName = roomCreateDto.RoomName,
                RoomAreaId = roomCreateDto.RoomAreaId,
                OrdinalNumber = roomCreateDto.OrdinalNumber,
                SeatNumber = roomCreateDto.SeatNumber,
                RoomNote = roomCreateDto.RoomNote,
                QrCodeUrl = null,
                Status = true,
                IsUse = false,
                IsDelete = false
            };
            await _roomRepository.AddAsync(room);
        }

        public async Task<string> GenerateQRCodeAndSaveAsync(int roomId, String UrlBase)
        {
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
            {
                throw new Exception("Room not found");
            }

            // Kiểm tra nếu room đã có QR Code URL
            if (!string.IsNullOrEmpty(room.QrCodeUrl))
            {
                throw new Exception("Room already has a QR Code");
            }

            // Dữ liệu để nhúng vào QR Code
            string qrData = $"{UrlBase}{roomId}";

            // Tạo và upload QR Code lên Cloudinary
            string qrCodeUrl = await _qrCodeService.GenerateAndUploadQRCodeAsync(qrData);

            // Lưu URL vào database
            await UpdateQRCodeUrlAsync(roomId, qrCodeUrl);

            return qrCodeUrl;
        }



        public async Task UpdateQRCodeUrlAsync(int roomId, string qrCodeUrl)
        {
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room != null)
            {
                room.QrCodeUrl = qrCodeUrl;
                await _roomRepository.UpdateAsync(room);
            }
        }
        public async Task DeleteQRCodeAsync(int roomId)
        {
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
            {
                throw new Exception("Room not found");
            }

            if (string.IsNullOrEmpty(room.QrCodeUrl))
            {
                throw new Exception("Room does not have a QR Code to delete");
            }

            // Xóa QR Code trên Cloudinary
            await _qrCodeService.DeleteQRCodeFromCloudinaryAsync(room.QrCodeUrl);

            // Xóa URL trong database
            room.QrCodeUrl = null;
            await _roomRepository.UpdateAsync(room);
        }

        public async Task<FileDataDto> DownloadQRCodeAsync(int roomId)
        {
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
            {
                throw new Exception("Room not found");
            }

            if (string.IsNullOrEmpty(room.QrCodeUrl))
            {
                throw new Exception("Room does not have a QR Code");
            }

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(room.QrCodeUrl);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to download QR Code");
                }

                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                string contentType = response.Content.Headers.ContentType?.ToString() ?? "image/png";

                return new FileDataDto
                {
                    Content = fileBytes,
                    ContentType = contentType,
                    FileName = $"QRCode_{room.RoomName}.png"
                };
            }
        }


        public async Task UpdateAsync(int id, RoomUpdateDto roomUpdateDto)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return;

            room.RoomName = roomUpdateDto.RoomName;
            room.RoomAreaId = roomUpdateDto.RoomAreaId;
            room.OrdinalNumber = roomUpdateDto.OrdinalNumber;
            room.SeatNumber = roomUpdateDto.SeatNumber;
            room.RoomNote = roomUpdateDto.RoomNote;
            room.Status = roomUpdateDto.Status;
            room.IsUse = roomUpdateDto.IsUse;
            room.IsDelete = roomUpdateDto.IsDelete ?? false;
            await _roomRepository.UpdateAsync(room);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            return await _roomRepository.SoftDeleteAsync(id);
        }

        public async Task<IEnumerable<RoomDto>> SearchByNameAsync(string name, int limit, int offset)
        {
            var rooms = await _roomRepository.SearchByNameAsync(name, limit, offset);

            return rooms.Select(r => new RoomDto
            {
                RoomId = r.RoomId,
                RoomName = r.RoomName,
                RoomAreaId = r.RoomAreaId,
                OrdinalNumber = r.OrdinalNumber,
                SeatNumber = r.SeatNumber,
                RoomNote = r.RoomNote,
                QrCodeUrl = r.QrCodeUrl,
                Status = r.Status,
                IsUse = r.IsUse,
                IsDelete = r.IsDelete
            }).ToList();
        }

        /* ======== Room Service - Thanh Tung ========= */

        // Service get room for POS site
        public async Task<List<RoomDtoPos>> GetRoomAsyncForPos()
        {
            var rooms = await _roomRepository.GetRoomsAsyncPOS();

            if (rooms == null)
            {
                throw new Exception("Error when take list room for POS");
            }

            return rooms.Select(r => new RoomDtoPos
            {
                RoomId = r.RoomId,
                RoomName = r.RoomName,
                RoomAreaId = r.RoomAreaId,
                OrdinalNumber = r.OrdinalNumber,
                SeatNumber = r.SeatNumber,
                RoomNote = r.RoomNote,
                IsUse = r.IsUse,
            }).ToList();
        }

        // Service of filter room by roomAreaId and isUse flag
        public async Task<List<RoomDtoPos>> FilterRoomAsyncPos(int? roomAreaId, bool? isUse)
        {

            if (roomAreaId <= 0)
            {
                throw new ArgumentOutOfRangeException("RoomAreaId must greater than 0");
            }

            var rooms = await _roomRepository.FilterRoomPosAsync(roomAreaId, isUse);

            return rooms.Select(r => new RoomDtoPos
            {
                RoomId = r.RoomId,
                RoomName = r.RoomName,
                RoomAreaId = r.RoomAreaId,
                OrdinalNumber = r.OrdinalNumber,
                SeatNumber = r.SeatNumber,
                RoomNote = r.RoomNote,
                IsUse = r.IsUse,
            }).ToList();
        }

        // Service for searching by username or room name
        //public async Task<List<RoomDtoPos>> SearchRoomPosAsync(string searchString)
        //{
        //    var rooms = await _roomRepository.SearchRoomPosAsync(searchString);

        //    return rooms.Select(r => new RoomDtoPos
        //    {
        //        RoomId = r.RoomId,
        //        RoomName = r.RoomName,
        //        RoomAreaId = r.RoomAreaId,
        //        OrdinalNumber = r.OrdinalNumber,
        //        RoomNote = r.RoomNote,
        //        IsUse = r.IsUse,
        //        OrderList = r.Orders.Select(o => new OrderDtoPos
        //        {
        //            OrderId = o.OrderId,
        //            CustomerId = o.CustomerId,
        //            CreatedTime = o.CreatedTime,
        //            AmountDue = o.AmountDue,
        //            OrderDetails = o.OrderDetails.Select(od => new OrderDetailDtoPos
        //            {
        //                OrderDetailId = od.OrderDetailId,
        //                ProductId = od.ProductId,
        //                Quantity = od.Quantity,
        //                Price = od.Price
        //            }).ToList() ?? new List<OrderDetailDtoPos>()
        //        }).ToList() ?? new List<OrderDtoPos>()
        //    }).ToList();
        //}



        //Pham Son Tung
        //Func to add note to room
        public async Task<bool> UpdateRoomNoteAsync(RoomNoteUpdateDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Request data cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(dto.RoomNote) || dto.RoomNote==null)
            {
                return await _roomRepository.UpdateRoomNote(dto.RoomId, "");
            }

            return await _roomRepository.UpdateRoomNote(dto.RoomId, dto.RoomNote);
        }

    }
}
