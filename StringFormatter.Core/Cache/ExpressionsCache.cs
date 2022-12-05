using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters;

namespace StringFormatter.Core.Cache
{
    public class ExpressionsCache
    {
        private readonly ConcurrentDictionary<string, Func<object, string>> _cache = new();
        private readonly ConcurrentDictionary<string, Func<object, int, string>> _collectionsCache = new();


        public string GetValue(string expression, object target)
        {
            var openBraceIndex = expression.IndexOf("[");
            var closeBraceIndex = expression.IndexOf("]");

            if(openBraceIndex != -1 && closeBraceIndex != -1)
            {
                return GetCollectionExpressionValue(expression, target);
            }
            else
            {
                return GetExpressionValue(expression, target);
            }

        }

        private string GetCollectionExpressionValue(string expression, object target)
        {
            var openBraceIndex = expression.IndexOf("[");
            var closeBraceIndex = expression.IndexOf("]");

            var collectionIdentifier = expression[..openBraceIndex];
            var index = int.Parse(expression[(openBraceIndex + 1)..closeBraceIndex]);

            var key = $"{target.GetType()}.{expression}";
            
            if (_collectionsCache.TryGetValue(key, out var func))
            {
                return func(target, index);
            }

            var propertyInfos = target.GetType().GetProperties();
            var fieldInfos = target.GetType().GetFields();

            if (propertyInfos.Any(p => p.Name == collectionIdentifier) ||
                    fieldInfos.Any(f => f.Name == collectionIdentifier))
            {   
                var targetPapam = Expression.Parameter(typeof(object), "target");
                var memberAccess = Expression.PropertyOrField(Expression.TypeAs(targetPapam, target.GetType()), collectionIdentifier);
                var indexParam = Expression.Parameter(typeof(int), "index");
                var arrayAccess = Expression.ArrayAccess(memberAccess, indexParam);
                var toStringExpression = Expression.Call(arrayAccess, "ToString", null, null);

                func = Expression.Lambda<Func<object, int, string>>(toStringExpression, targetPapam, indexParam).Compile();

                _collectionsCache.TryAdd(key, func);
            }
            else
            {
                throw new Exception($"Invalid property/field name {collectionIdentifier}");
            }

            return func(target, index);
        }

        private string GetExpressionValue(string expression, object target)
        {
            var key = $"{target.GetType()}.{expression}";

            if(_cache.TryGetValue(key, out var func))
            {
                return func(target);
            }

            var propertyInfos = target.GetType().GetProperties();
            var fieldInfos = target.GetType().GetFields();

            if (propertyInfos.Any(p => p.Name == expression) ||
                    fieldInfos.Any(f => f.Name == expression))
            {
                var targetParam = Expression.Parameter(typeof(object), "target");
                var memberAccess = Expression.PropertyOrField(Expression.TypeAs(targetParam, target.GetType()), expression);
                var toStringExpression = Expression.Call(memberAccess, "ToString", null, null);
                func = Expression.Lambda<Func<object, string>>(toStringExpression, targetParam).Compile();

                _cache.TryAdd(key, func);
            }
            else
            {
                throw new Exception($"Invalid property/field name {expression}");
            }

            return func(target);
        }
    }
}
