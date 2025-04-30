using BepKhoiBackend.BusinessObject.dtos.UnitDto;

namespace BepKhoiBackend.BusinessObject.Abstract.UnitAbstract
{
    public interface IUnitService
    {
        Task<IEnumerable<UnitDto>> GetUnitsAsync();
    }
}
