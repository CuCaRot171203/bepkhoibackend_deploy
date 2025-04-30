using BepKhoiBackend.BusinessObject.dtos.RoomAreaDto;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository.RoomAreaRepository.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Services.RoomAreaService
{
    public class RoomAreaService : IRoomAreaService
    {
        private readonly IRoomAreaRepository _roomAreaRepository;

        public RoomAreaService(IRoomAreaRepository roomAreaRepository)
        {
            _roomAreaRepository = roomAreaRepository;
        }

        public async Task<IEnumerable<RoomAreaDto>> GetAllAsync(int limit, int offset)
        {
            var roomAreas = await _roomAreaRepository.GetAllAsync(limit, offset);
            return roomAreas.Select(r => new RoomAreaDto
            {
                RoomAreaId = r.RoomAreaId,
                RoomAreaName = r.RoomAreaName,
                RoomAreaNote = r.RoomAreaNote,
                IsDelete = r.IsDelete ?? false  // Fix lỗi null
                
            }).ToList();
        }

        public async Task<RoomAreaDto> GetByIdAsync(int id)
        {
            var roomArea = await _roomAreaRepository.GetByIdAsync(id);
            if (roomArea == null) return null;

            return new RoomAreaDto
            {
                RoomAreaId = roomArea.RoomAreaId,
                RoomAreaName = roomArea.RoomAreaName,
                RoomAreaNote = roomArea.RoomAreaNote,
                IsDelete = roomArea.IsDelete ?? false
            };
        }

        public async Task AddAsync(RoomAreaDto roomAreaDto)
        {
            var roomArea = new RoomArea
            {
                RoomAreaName = roomAreaDto.RoomAreaName,
                RoomAreaNote = roomAreaDto.RoomAreaNote,
                IsDelete = false  // Mặc định chưa bị xóa
            };

            await _roomAreaRepository.AddAsync(roomArea);
        }

        public async Task UpdateAsync(RoomAreaDto roomAreaDto)
        {
            var roomArea = await _roomAreaRepository.GetByIdAsync(roomAreaDto.RoomAreaId);
            if (roomArea == null) return;

            roomArea.RoomAreaName = roomAreaDto.RoomAreaName;
            roomArea.RoomAreaNote = roomAreaDto.RoomAreaNote;
            roomArea.IsDelete = roomAreaDto.IsDelete ?? false;

            await _roomAreaRepository.UpdateAsync(roomArea);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var roomArea = await _roomAreaRepository.GetByIdAsync(id);
            if (roomArea == null) return false;

            roomArea.IsDelete = true;
            await _roomAreaRepository.UpdateAsync(roomArea);
            return true;
        }
        public async Task<IEnumerable<RoomAreaDto>> GetByIdAndIsDeleteAsync(bool isDelete, int limit, int offset)
        {
            var roomAreas = await _roomAreaRepository.GetByIdAndIsDeleteAsync(isDelete, limit, offset);
            return roomAreas.Select(r => new RoomAreaDto
            {
                RoomAreaId = r.RoomAreaId,
                RoomAreaName = r.RoomAreaName,
                RoomAreaNote = r.RoomAreaNote,
                IsDelete = r.IsDelete ?? false
            }).ToList();
        }
        public async Task<IEnumerable<RoomAreaDto>> SearchByNameOrIdAsync(string name, int limit, int offset)
        {
            var roomAreas = await _roomAreaRepository.SearchByNameOrIdAsync(name, limit, offset);
            return roomAreas.Select(r => new RoomAreaDto
            {
                RoomAreaId = r.RoomAreaId,
                RoomAreaName = r.RoomAreaName,
                RoomAreaNote = r.RoomAreaNote,
                IsDelete = r.IsDelete ?? false
            }).ToList();
        }



    }
}
