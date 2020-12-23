using AutoMapper;
using System.Collections.Generic;

namespace ProductDb.Mapping.AutoMapperConfigurations
{
    public class AutoMapperConfiguration : IAutoMapperConfiguration
    {
        private readonly IMapper mapper;
        private readonly MapperConfiguration configuration;

        public AutoMapperConfiguration()
        {
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps("ProductDb.Mapping");
            });

            mapper = new Mapper(configuration);
        }


        public TReturn MapObject<TMap, TReturn>(TMap obj) where TMap : class where TReturn : class
        {
            return mapper.Map<TMap, TReturn>(obj);
        }

        public IEnumerable<TReturn> MapCollection<TMap, TReturn>(IEnumerable<TMap> expression) where TMap : class where TReturn : class
        {
            return mapper.Map<IEnumerable<TMap>, IEnumerable<TReturn>>(expression);
        }
    }
}
