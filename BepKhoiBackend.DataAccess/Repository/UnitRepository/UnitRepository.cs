using BepKhoiBackend.DataAccess.Abstract.UnitAbstract;
using BepKhoiBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace BepKhoiBackend.DataAccess.Repository.UnitRepository
{
    public class UnitRepository : IUnitRepository
    {
        private readonly bepkhoiContext _context;

        public UnitRepository(bepkhoiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Unit>> GetUnitsAsync()
        {
            return await _context.Units
                                 .Where(u => u.IsDelete == false || u.IsDelete == null)
                                 .ToListAsync();
        }
    }
}
