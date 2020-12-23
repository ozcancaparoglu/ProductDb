using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Mapping.AutoMapperConfiguration
{
    public class AutoMapperService : IAutoMapperService
    {
        private readonly IMapper Mapper;
        public AutoMapperService()
        {
            var cfg = new MapperConfiguration(a =>
            {
                a.AddProfile(typeof(MappingProfile));
            });

            Mapper = new Mapper(cfg);
        }
        public TReturn Map<TMap, TReturn>(TMap Map) 
            where TMap : class where TReturn : class
        {
            return Mapper.Map<TReturn>(Map);
        }

        public IEnumerable<TReturn> MapCollection<TMap, TReturn>(IEnumerable<TMap> Map) 
            where TMap : class where TReturn : class
        {
            return Mapper.Map<IEnumerable<TReturn>>(Map);
        }
    }
}
