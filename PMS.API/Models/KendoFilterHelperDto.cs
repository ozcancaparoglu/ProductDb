using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PMS.API.Models
{
    public class KendoFilterHelperDto
    {
        public static IEnumerable<T> FilterQueryableData<T>(IQueryable<T> datas, KendoFilterDto kendoFilters) where T : class
        {
            if (kendoFilters.filter != null)
            {
                Expression<Func<T, bool>> predicate = null;

                foreach (var filter in kendoFilters.filter.filters)
                {

                    Enum.TryParse(filter.@operator, out KendoFilterEnumDto filterOperator);
                    var value = filter.value.Trim();
                    predicate = CreatePredicate<T>(filter.field, value, filterOperator);

                    datas = datas.Where(predicate);
                }
            }
            
            return datas;
        }

        public static Expression<Func<T, bool>> CreatePredicate<T>(string columnName, object searchValue, KendoFilterEnumDto kendoOperator) where T : class
        {
            var xType = typeof(T);
            var x = Expression.Parameter(xType, "type");
            var column = xType.GetProperties().FirstOrDefault(p => p.Name.ToLowerInvariant() == columnName.ToLowerInvariant());

            Expression body = null;

            switch (kendoOperator)
            {
                case KendoFilterEnumDto.isequalto:
                    body = column == null
                ? (Expression)Expression.Constant(true)
                : Expression.Equal(
                    Expression.PropertyOrField(x, columnName),
                    Expression.Constant(searchValue));
                    break;
                case KendoFilterEnumDto.isnotequalto:
                    body = column == null
                 ? (Expression)Expression.Constant(true)
                 : Expression.NotEqual(
                     Expression.PropertyOrField(x, columnName),
                     Expression.Constant(searchValue));
                    break;
                case KendoFilterEnumDto.startswith:
                    body = column == null
                 ? (Expression)Expression.Constant(true)
                 : Expression.Call(Expression.Property(x, column.Name), typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), Expression.Constant(searchValue, typeof(string)));
                    break;
                case KendoFilterEnumDto.contains:
                    body = column == null
                 ? (Expression)Expression.Constant(true)
                 : Expression.Call(Expression.Property(x, column.Name), typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(searchValue, typeof(string)));
                    break;
                case KendoFilterEnumDto.doesnotcontain:
                    body = column == null
                ? (Expression)Expression.Constant(true)
                : Expression.Not(Expression.Constant(searchValue, typeof(string)), typeof(string).GetMethod("Contains", new[] { typeof(string) }));
                    break;
                case KendoFilterEnumDto.endswith:
                    body = column == null
                 ? (Expression)Expression.Constant(true)
                 : Expression.Call(Expression.Property(x, column.Name), typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), Expression.Constant(searchValue, typeof(string)));
                    break;
                case KendoFilterEnumDto.isnull:
                    body = column == null
                ? (Expression)Expression.Constant(true)
                : Expression.Equal(
                    Expression.PropertyOrField(x, columnName),
                    Expression.Constant(null, typeof(object)));
                    break;
                case KendoFilterEnumDto.isnotnull:
                    body = column == null
                ? (Expression)Expression.Constant(true)
                : Expression.NotEqual(
                    Expression.PropertyOrField(x, columnName),
                    Expression.Constant(null, typeof(object)));
                    break;
                case KendoFilterEnumDto.isempty:
                    body = column == null
                 ? (Expression)Expression.Constant(true)
                 : Expression.Equal(
                     Expression.PropertyOrField(x, columnName),
                     Expression.Constant(string.Empty, typeof(object)));
                    break;
                case KendoFilterEnumDto.isnotempty:
                    body = column == null
              ? (Expression)Expression.Constant(true)
              : Expression.NotEqual(
                  Expression.PropertyOrField(x, columnName),
                  Expression.Constant(string.Empty, typeof(object)));
                    break;
                case KendoFilterEnumDto.hasnovalue:
                    body = column == null
               ? (Expression)Expression.Constant(true)
               : Expression.Equal(
                   Expression.PropertyOrField(x, columnName),
                   Expression.Constant(null, typeof(object)));
                    break;
                case KendoFilterEnumDto.hasvalue:
                    body = column == null
              ? (Expression)Expression.Constant(true)
              : Expression.Equal(
                  Expression.PropertyOrField(x, columnName),
                  Expression.Constant(null, typeof(object)));
                    break;
                default:
                    break;
            }

            return Expression.Lambda<Func<T, bool>>(body, x);
        }
    }
}
