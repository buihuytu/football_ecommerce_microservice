using AutoMapper;

namespace Catalog.Application.Mappers
{
    public class ProductMapper
    {
        // This class can be used to define mapping configurations if needed in the future.
        // Currently, it is empty as the mapping is handled by AutoMapper profiles.
        private static readonly Lazy<IMapper> Lazy = new(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly; // Include non-public fields if needed
                cfg.AddProfile<ProductMappingProfile>();
            });
            var mapper = config.CreateMapper();
            return mapper;
        });

        public static IMapper Mapper => Lazy.Value;
    }
}
