using BepKhoiBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Repository.RoomAreaRepository.Interface
{
    public interface IRoomAreaRepository
    {
        Task<IEnumerable<RoomArea>> GetAllAsync(int limit, int offset);
        Task<RoomArea> GetByIdAsync(int id);
        Task AddAsync(RoomArea roomArea);
        Task UpdateAsync(RoomArea roomArea);
        Task<bool> SoftDeleteAsync(int id);
        public Task<IEnumerable<RoomArea>> GetByIdAndIsDeleteAsync(bool isDelete, int limit, int offset);
        public Task<IEnumerable<RoomArea>> SearchByNameOrIdAsync(string name, int limit, int offset);
    }
}
