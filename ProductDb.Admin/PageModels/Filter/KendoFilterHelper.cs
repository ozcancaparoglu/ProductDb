using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace ProductDb.Admin.PageModels.Filter
{
    public static class KendoFilterHelper
    {
        public static IEnumerable<T> FilterQueryableData<T>(IQueryable<T> datas, KendoFilterModel kendoFilters) where T : class
        {

            Expression<Func<T, bool>> predicate = null;

            foreach (var filter in kendoFilters.filter.filters)
            {

                Enum.TryParse(filter.@operator, out KendoOperatorEnum filterOperator);
                var value = filter.value.Trim();
                predicate = CreatePredicate<T>(filter.field, value, filterOperator);

                datas = datas.Where(predicate);
            }

            return datas;
        }

        public static Expression<Func<T, bool>> CreatePredicate<T>(string columnName, object searchValue, KendoOperatorEnum kendoOperator) where T : class
        {
            var xType = typeof(T);
            var x = Expression.Parameter(xType, "type");
            var column = xType.GetProperties().FirstOrDefault(p => p.Name.ToLowerInvariant() == columnName.ToLowerInvariant());

            Expression body = null;

            switch (kendoOperator)
            {
                case KendoOperatorEnum.isequalto:
                    body = column == null
                ? (Expression)Expression.Constant(true)
                : Expression.Equal(
                    Expression.PropertyOrField(x, columnName),
                    Expression.Constant(searchValue));
                    break;
                case KendoOperatorEnum.isnotequalto:
                    body = column == null
                 ? (Expression)Expression.Constant(true)
                 : Expression.NotEqual(
                     Expression.PropertyOrField(x, columnName),
                     Expression.Constant(searchValue));
                    break;
                case KendoOperatorEnum.startswith:
                    body = column == null
                 ? (Expression)Expression.Constant(true)
                 : Expression.Call(Expression.Property(x, column.Name), typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), Expression.Constant(searchValue, typeof(string)));
                    break;
                case KendoOperatorEnum.contains:
                    body = column == null
                 ? (Expression)Expression.Constant(true)
                 : Expression.Call(Expression.Property(x, column.Name), typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(searchValue, typeof(string)));
                    break;
                case KendoOperatorEnum.doesnotcontain:
                    body = column == null
                ? (Expression)Expression.Constant(true)
                : Expression.Not(Expression.Constant(searchValue, typeof(string)), typeof(string).GetMethod("Contains", new[] { typeof(string) }));
                    break;
                case KendoOperatorEnum.endswith:
                    body = column == null
                 ? (Expression)Expression.Constant(true)
                 : Expression.Call(Expression.Property(x, column.Name), typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), Expression.Constant(searchValue, typeof(string)));
                    break;
                case KendoOperatorEnum.isnull:
                    body = column == null
                ? (Expression)Expression.Constant(true)
                : Expression.Equal(
                    Expression.PropertyOrField(x, columnName),
                    Expression.Constant(null, typeof(object)));
                    break;
                case KendoOperatorEnum.isnotnull:
                    body = column == null
                ? (Expression)Expression.Constant(true)
                : Expression.NotEqual(
                    Expression.PropertyOrField(x, columnName),
                    Expression.Constant(null, typeof(object)));
                    break;
                case KendoOperatorEnum.isempty:
                    body = column == null
                 ? (Expression)Expression.Constant(true)
                 : Expression.Equal(
                     Expression.PropertyOrField(x, columnName),
                     Expression.Constant(string.Empty, typeof(object)));
                    break;
                case KendoOperatorEnum.isnotempty:
                    body = column == null
              ? (Expression)Expression.Constant(true)
              : Expression.NotEqual(
                  Expression.PropertyOrField(x, columnName),
                  Expression.Constant(string.Empty, typeof(object)));
                    break;
                case KendoOperatorEnum.hasnovalue:
                    body = column == null
               ? (Expression)Expression.Constant(true)
               : Expression.Equal(
                   Expression.PropertyOrField(x, columnName),
                   Expression.Constant(null, typeof(object)));
                    break;
                case KendoOperatorEnum.hasvalue:
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

        public static IEnumerable<T> FilterListData<T>(IEnumerable<T> datas, KendoFilterModel kendoFilters) where T : class
        {
            foreach (var filter in kendoFilters.filter.filters)
            {
                Enum.TryParse(filter.@operator, out KendoOperatorEnum filterOperator);
                switch (filterOperator)
                {
                    case KendoOperatorEnum.isequalto:
                        datas = datas.Where(x => x.GetType().GetProperty(filter.field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(x).ToString() == filter.value);
                        break;
                    case KendoOperatorEnum.isnotequalto:
                        datas = datas.Where(x => x.GetType().GetProperty(filter.field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(x).ToString() != filter.value);
                        break;
                    case KendoOperatorEnum.startswith:
                        datas = datas.Where(x => x.GetType().GetProperty(filter.field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(x).ToString().StartsWith(filter.value));
                        break;
                    case KendoOperatorEnum.contains:
                        datas = datas.Where(x => x.GetType().GetProperty(filter.field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(x).ToString().Contains(filter.value));
                        break;
                    case KendoOperatorEnum.doesnotcontain:
                        var contains = datas.Where(x => x.GetType().GetProperty(filter.field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(x).ToString().Contains(filter.value));
                        datas = datas.Except(contains);
                        break;
                    case KendoOperatorEnum.endswith:
                        datas = datas.Where(x => x.GetType().GetProperty(filter.field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(x).ToString().EndsWith(filter.value));
                        break;
                    case KendoOperatorEnum.isnull:
                        datas = datas.Where(x => x.GetType().GetProperty(filter.field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(x).ToString() == null);
                        break;
                    case KendoOperatorEnum.isnotnull:
                        datas = datas.Where(x => x.GetType().GetProperty(filter.field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(x).ToString() != null);
                        break;
                    case KendoOperatorEnum.isempty:
                        datas = datas.Where(x => x.GetType().GetProperty(filter.field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(x).ToString() == string.Empty);
                        break;
                    case KendoOperatorEnum.isnotempty:
                        datas = datas.Where(x => x.GetType().GetProperty(filter.field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(x).ToString() != string.Empty);
                        break;
                    case KendoOperatorEnum.hasnovalue:
                        datas = datas.Where(x => x.GetType().GetProperty(filter.field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(x).ToString() != string.Empty);
                        break;
                    case KendoOperatorEnum.hasvalue:
                        break;
                    default:
                        break;
                }
            }
            return datas;
        }
    }
}
