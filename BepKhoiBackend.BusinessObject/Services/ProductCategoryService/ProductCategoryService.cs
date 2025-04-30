using AutoMapper;
using BepKhoiBackend.BusinessObject.Abstract.ProductCategoryAbstract;
using BepKhoiBackend.BusinessObject.dtos.ProductCategoryDto;
using BepKhoiBackend.DataAccess.Abstract.ProductCategoryAbstract;

namespace BepKhoiBackend.BusinessObject.Services.ProductCategoryService
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IMapper _mapper;

        public ProductCategoryService(IProductCategoryRepository productCategoryRepository, IMapper mapper)
        {
            _productCategoryRepository = productCategoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductCategoryDto>> GetProductCategoriesAsync()
        {
            var categories = await _productCategoryRepository.GetProductCategoriesAsync();
            return _mapper.Map<IEnumerable<ProductCategoryDto>>(categories);
        }
    }
}
