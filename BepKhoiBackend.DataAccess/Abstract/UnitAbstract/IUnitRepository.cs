using BepKhoiBackend.DataAccess.Models;

namespace BepKhoiBackend.DataAccess.Abstract.UnitAbstract
{
    public interface IUnitRepository
    {
        Task<IEnumerable<Unit>> GetUnitsAsync();
    }
}
