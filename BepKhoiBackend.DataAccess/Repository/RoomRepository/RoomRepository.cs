using BepKhoiBackend.DataAccess.Helpers;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository.RoomRepository.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Repository.RoomRepository
{
    public class RoomRepository : IRoomRepository
    {
        private readonly bepkhoiContext _context;

        public RoomRepository(bepkhoiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllAsync(int limit, int offset)
        {
            return await _context.Rooms
                .Where(r => r.IsDelete == false)
                .OrderBy(r => r.RoomId)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Room> GetByIdAsync(int id)
        {
            return await _context.Rooms
                .FirstOrDefaultAsync(r => r.RoomId == id && r.IsDelete == false);
        }

        public async Task<IEnumerable<Room>> SearchByIdOrNameAsync(string keyword)
        {
            return await _context.Rooms
                .Where(r => r.IsDelete == false &&
                            (r.RoomId.ToString() == keyword || r.RoomName.Contains(keyword)))
                .ToListAsync();
        }

        public async Task AddAsync(Room room)
        {
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            bool hasOrders = await _context.Orders.AnyAsync(o => o.RoomId == id);
            if (hasOrders)
            {
                return false;
            }

            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == id);
            if (room == null) return false;

            room.IsDelete = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Room>> SearchByNameAsync(string name, int limit, int offset)
        {
            var query = _context.Rooms
                .Where(r => r.IsDelete == false);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(r => r.RoomName.Contains(name));
            }

            return await query
                .OrderBy(r => r.RoomId)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        // ========== Room Repository - Thanh Tung ============

        // Repository of get room for POS site
        public async Task<List<Room>> GetRoomsAsyncPOS()
        {
            try
            {
                return await _context.Rooms
                    .AsNoTracking()
                    .Where(r => r.IsDelete==false && r.Status==true)
                    .OrderBy(r => r.OrdinalNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error when querry from database.", ex);
            }
        }

        // Repository of filter room pos async 
        public async Task<List<Room>> FilterRoomPosAsync(int? roomAreaId, bool? isUse)
        {
            try
            {
                var query = _context.Rooms
                    .AsNoTracking()
                    .Where(r => r.IsDelete==false && r.Status==true);

                if (roomAreaId.HasValue)
                    query = query.Where(r => r.RoomAreaId == roomAreaId.Value);

                if (isUse.HasValue)
                    query = query.Where(r => r.IsUse == isUse.Value);

                return await query
                    .OrderBy(r => r.OrdinalNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error when filter from database.", ex);
            }
        }

        // Repository of search room POS async
        public async Task<List<Room>> SearchRoomPosAsync(string searchString)
        {
            try
            {
                // Validate input
                StringHelper.ValidateSearchInput(searchString);

                // Normalize input (remove diacritics)
                string normalizedKeyword = StringHelper.RemoveDiacritics(searchString).ToLower();

                var rooms = await _context.Rooms
                    .AsNoTracking()
                    .Where(r => !(r.IsDelete ?? false))
                    .OrderBy(r => r.RoomId)
                    .Include(r => r.Orders.Where(o => o.OrderStatusId == 1))
                    .ThenInclude(o => o.OrderDetails)
                    .ToListAsync();

                var filteredRooms = rooms.Where(r =>
                    StringHelper.RemoveDiacritics(r.RoomName).ToLower().Contains(normalizedKeyword) ||
                    r.Orders.Any(o => o.Customer != null &&
                        StringHelper.RemoveDiacritics(o.Customer.CustomerName).ToLower().Contains(normalizedKeyword))
                ).ToList();

                return filteredRooms;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when searching from database: {ex.Message}", ex);
            }
        }

        //Pham Son Tung
        //Func to add note to room
        public async Task<bool> UpdateRoomNote(int roomId, string roomNote)
        {
            try
            {
                var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId);
                if (room == null)
                {
                    throw new KeyNotFoundException($"Room with ID {roomId} not found.");
                }

                room.RoomNote = roomNote;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                // Ném lỗi nếu có vấn đề trong quá trình cập nhật cơ sở dữ liệu
                throw new InvalidOperationException($"Database update error: {dbEx.Message}", dbEx);
            }
            catch (SqlException sqlEx)
            {
                // Ném lỗi nếu có vấn đề với kết nối hoặc truy vấn SQL
                throw new InvalidOperationException($"SQL error: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                // Ném lỗi nếu có lỗi không xác định
                throw new InvalidOperationException($"Unexpected error: {ex.Message}", ex);
            }
        }


    }
}
