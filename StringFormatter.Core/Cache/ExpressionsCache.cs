using System.Collections.Concurrent;
using System.Reflection;

namespace StringFormatter.Core.Cache
{
    public class ExpressionsCache
    {
        private readonly ConcurrentDictionary<string, Func<object, string>> _cache = new();
        private readonly ConcurrentDictionary<string, Func<object, int, string>> _collectionsCache = new();

        private PropertyInfo[] _propertyInfos;
        private FieldInfo[] _fieldInfos;

        public ExpressionsCache(object target) 
        {
            _propertyInfos = target.GetType().GetProperties();
            _fieldInfos = target.GetType().GetFields();
        }

        /*public bool TryGetValue(string identifier, out string stringValue)
        {
            var openBraceIndex = identifier.IndexOf("[", StringComparison.Ordinal);
            var closeBraceIndex = 0;

        }*/


    }
}
