using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Mapping.AutoMapperConfiguration
{
    public interface IAutoMapperService
    {
        TReturn Map<TMap, TReturn>(TMap Map) 
            where TMap: class where TReturn : class;
        IEnumerable<TReturn> MapCollection<TMap, TReturn>(IEnumerable<TMap> Map)
            where TMap : class where TReturn : class;
    }
}
