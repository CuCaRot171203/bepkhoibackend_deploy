using BepKhoiBackend.BusinessObject.dtos.RoomAreaDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Services.RoomAreaService
{
    public interface IRoomAreaService
    {
        Task<IEnumerable<RoomAreaDto>> GetAllAsync(int limit, int offset);
        Task<RoomAreaDto> GetByIdAsync(int id);
        Task AddAsync(RoomAreaDto roomAreaDto);
        Task UpdateAsync(RoomAreaDto roomAreaDto);
        Task<bool> SoftDeleteAsync(int id);
        public Task<IEnumerable<RoomAreaDto>> GetByIdAndIsDeleteAsync(bool isDelete, int limit, int offset);
        public Task<IEnumerable<RoomAreaDto>> SearchByNameOrIdAsync(string name, int limit, int offset);

    }
}
