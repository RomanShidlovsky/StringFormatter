using System.Text;

namespace StringFormatter.Core.Parser.Context
{
    public class ParserContext
    {
        public object Target { get; init; }
        public StringBuilder ResultBuilder { get; init; }
        public StringBuilder IdentifierNameBuilder { get; init; }

        public int OpenCurlyBraceCount { get; set; }
        public int CloseCurlyBraceCount { get; set; }

        public ParserContext(object target)
        {
            Target = target;
            ResultBuilder= new StringBuilder();
            IdentifierNameBuilder = new StringBuilder();
            OpenCurlyBraceCount = 0;
            CloseCurlyBraceCount = 0;
        }

    }
}
