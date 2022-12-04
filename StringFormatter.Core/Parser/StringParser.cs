using StringFormatter.Core.Interfaces;
using StringFormatter.Core.Parser.Context;

namespace StringFormatter.Core.Parser
{
    public class StringParser : IStringParser
    {
        private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Numbers = "0123456789";
        private const char Underscore = '_';
        private const char OpenCurlyBrace = '{';
        private const char CloseCurlyBrace = '}';

        private delegate void StateMatrixDelegate(char symbol, ParserContext context);

        private StateMatrixDelegate[,] _stateMatrix = new StateMatrixDelegate[5, 5]
        {
            {ErrorState, ErrorState, ErrorState, ErrorState, ErrorState },
            {ErrorState, State11,    State12,    State13,    ErrorState },
            {ErrorState, State21,    ErrorState, ErrorState, State24 },
            {ErrorState, State31,    ErrorState, ErrorState, ErrorState },
            {ErrorState, State41,    ErrorState, ErrorState, State44 }
        };
        
        public string Parse(string template, object target)
        {
            var context = new ParserContext(target);
            var state = 1;
            var nextState = 1;

            for(int i = 0; i < template.Length; i++)
            {
                char symbol = template[i];

                switch (state)
                {
                    case 1:
                        if (symbol == OpenCurlyBrace)
                            nextState= 2;
                        else if (symbol == CloseCurlyBrace)
                            nextState = 3;
                        else
                            nextState = 1;

                        _stateMatrix[state, nextState](symbol, context);
                        state = nextState;
                        break;

                    case 2:
                        if (symbol == OpenCurlyBrace)
                            nextState = 1;
                        else if (Letters.Contains(char.ToUpper(symbol)) || symbol == Underscore)
                            nextState = 4;
                        else
                            throw new Exception($"Invalid identifier name starts with {symbol} at {i} position");

                        _stateMatrix[state, nextState](symbol, context);
                        state = nextState; 
                        break;

                    case 3:
                        if (symbol == CloseCurlyBrace)
                            nextState = 1;
                        else
                            throw new Exception($"Closing curly brace without corresponging open curling brace at position {i}");

                        _stateMatrix[state, nextState](symbol, context);
                        state = nextState;
                        break;

                    case 4:
                        if (symbol == CloseCurlyBrace)
                            nextState = 1;
                        else if (symbol == Underscore || Numbers.Contains(symbol) || Letters.Contains(char.ToUpper(symbol)))
                            nextState = 4;
                        else
                            throw new Exception($"Invalid character in identifier name at position {i}");
                        
                        _stateMatrix[state, nextState](symbol, context);
                        state = nextState;
                        break;
                }
            }

            if (context.OpenCurlyBraceCount != context.CloseCurlyBraceCount)
            {
                throw new Exception("Amount of opening curly braces does not match amount of closing");
            }

            return context.ResultBuilder.ToString();
        }

        private static void ErrorState(char symbol, ParserContext context)
        {
            throw new Exception("Invalid string format");
        }

        private static void State11(char symbol, ParserContext context)
        {
            context.ResultBuilder.Append(symbol);
        }

        private static void State12(char symbol, ParserContext context)
        {
            //_context.OpenCurlyBraceCount++;
        }

        private static void State13(char symbol, ParserContext context)
        {
            context.CloseCurlyBraceCount++;
        }

        private static void State21(char symbol, ParserContext context)
        {
            context.ResultBuilder.Append(symbol);
            context.OpenCurlyBraceCount = context.OpenCurlyBraceCount - 2;
        }

        private static void State24(char symbol, ParserContext context)
        {
            context.IdentifierNameBuilder.Clear();
            context.IdentifierNameBuilder.Append(symbol);
        }

        private static void State31(char symbol, ParserContext context)
        {
            context.ResultBuilder.Append(symbol);
        }

        private static void State41(char symbol, ParserContext context)
        {
            // add identifier to cash
            // add value of field to result
            context.ResultBuilder.Append("FIELD_VALUE");
            context.CloseCurlyBraceCount++;
        }

        private static void State44(char symbol, ParserContext context)
        {
            context.IdentifierNameBuilder.Append(symbol);
        }
    }
}
