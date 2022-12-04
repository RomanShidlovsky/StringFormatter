using System.Text;

namespace StringFormatter.Core.Parser.Context
{
    public class ParserContext
    {
        public object Target { get; init; }
        public StringBuilder ResultBuilder { get; init; }
        public StringBuilder ExpressionBuilder { get; init; }

        public int OpenCurlyBraceCount { get; set; }
        public int CloseCurlyBraceCount { get; set; }

        public ParserContext(object target)
        {
            Target = target;
            ResultBuilder= new StringBuilder();
            ExpressionBuilder = new StringBuilder();
            OpenCurlyBraceCount = 0;
            CloseCurlyBraceCount = 0;
        }

    }
}
