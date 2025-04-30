using AutoMapper;
using BepKhoiBackend.BusinessObject.Abstract.UnitAbstract;
using BepKhoiBackend.BusinessObject.dtos.UnitDto;
using BepKhoiBackend.DataAccess.Abstract.UnitAbstract;

namespace BepKhoiBackend.BusinessObject.Services.UnitService
{
    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IMapper _mapper;

        public UnitService(IUnitRepository unitRepository, IMapper mapper)
        {
            _unitRepository = unitRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UnitDto>> GetUnitsAsync()
        {
            var units = await _unitRepository.GetUnitsAsync();
            return _mapper.Map<IEnumerable<UnitDto>>(units);
        }
    }
}
