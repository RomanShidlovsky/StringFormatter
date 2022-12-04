using StringFormatter.Core.Interfaces;
using StringFormatter.Core.Parser;

namespace StringFormatter.Core
{
    public class StringFormatter : IStringFormatter
    {
        public static readonly StringFormatter Shared = new StringFormatter();
        private IStringParser _parser;

        public StringFormatter() 
        {
            _parser = new StringParser();
        }

        public StringFormatter(IStringParser parser)
        {
            _parser = parser;
        }

        public string Format(string template, object target)
        {
            return _parser.Parse(template, target);
        }
    }
}
