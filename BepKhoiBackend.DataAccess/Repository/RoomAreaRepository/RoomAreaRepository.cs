using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository.RoomAreaRepository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Repository.RoomAreaRepository
{
    public class RoomAreaRepository : IRoomAreaRepository
    {
        private readonly bepkhoiContext _context;

        public RoomAreaRepository(bepkhoiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoomArea>> GetAllAsync(int limit, int offset)
        {
            return await _context.RoomAreas
                .Where(r => r.IsDelete == false)  // Fix lỗi where clause
                .OrderBy(r => r.RoomAreaId)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<RoomArea> GetByIdAsync(int id)
        {
            return await _context.RoomAreas.FirstOrDefaultAsync(r => r.RoomAreaId == id && r.IsDelete == false);
        }

        public async Task AddAsync(RoomArea roomArea)
        {
            await _context.RoomAreas.AddAsync(roomArea);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RoomArea roomArea)
        {
            _context.RoomAreas.Update(roomArea);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var roomArea = await _context.RoomAreas.FindAsync(id);
            if (roomArea == null) return false;

            roomArea.IsDelete = true;
            _context.RoomAreas.Update(roomArea);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RoomArea>> GetByIdAndIsDeleteAsync(bool isDelete, int limit, int offset)
        {
            return await _context.RoomAreas
                .Where(r =>   r.IsDelete == isDelete)
                .OrderBy(r => r.RoomAreaId)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<RoomArea>> SearchByNameOrIdAsync(string name, int limit, int offset)
        {
            var query = _context.RoomAreas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(r => r.RoomAreaName.Contains(name.Trim()) && r.IsDelete==false);
            }
            return await query
                .OrderBy(r => r.RoomAreaId)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }
    }
}
