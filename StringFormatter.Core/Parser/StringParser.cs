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

        private delegate void StateMatrixDelegate(char character, ParserContext context);

        private StateMatrixDelegate[,] _stateMatrix = new StateMatrixDelegate[7, 7]
        {
            { ErrorState, ErrorState, ErrorState, ErrorState, ErrorState, ErrorState, ErrorState },
            { ErrorState, State11,    State12,    State13,    ErrorState, ErrorState, ErrorState },
            { ErrorState, State21,    ErrorState, ErrorState, State24,    ErrorState, ErrorState },
            { ErrorState, State31,    ErrorState, ErrorState, ErrorState, ErrorState, ErrorState },
            { ErrorState, State41,    ErrorState, ErrorState, State44,    State45,    ErrorState },
            { ErrorState, ErrorState, ErrorState, ErrorState, ErrorState, State55,    State56    },
            { ErrorState, State61,    ErrorState, ErrorState, ErrorState, ErrorState, ErrorState }
        };
        
        public string Parse(string template, object target)
        {
            var context = new ParserContext(target);
            var state = 1;
            var nextState = 1;

            for(int i = 0; i < template.Length; i++)
            {
                /*char character = template[i];
                char nextcharacter*/

                var character = template[i];

                switch (state)
                {
                    case 1:
                        if (character == '{')
                            nextState = 2;
                        else if (character == '}')
                            nextState = 3;
                        else
                            nextState = 1;

                        _stateMatrix[state, nextState](character, context);
                        state = nextState;
                        break;

                    case 2: 
                        if (character == '{')
                            nextState = 1;
                        else if (Letters.Contains(char.ToUpper(character)) || character == '_')
                            nextState = 4;
                        else
                            throw new Exception($"Invalid identifier name can`t start with {character} at {i} position");

                        _stateMatrix[state, nextState](character, context);
                        state = nextState; 
                        break;

                    case 3:
                        if (character == '}')
                            nextState = 1;
                        else
                            throw new Exception($"A character '}}' must be escaped by doubling");

                        _stateMatrix[state, nextState](character, context);
                        state = nextState;
                        break;

                    case 4:
                        if (character == '}')
                            nextState = 1;
                        else if (character == '_' || Numbers.Contains(character) || Letters.Contains(char.ToUpper(character)))
                            nextState = 4;
                        else if (character == '[')
                            nextState = 5;
                        else
                            throw new Exception($"Invalid character in identifier name at position {i}");
                        
                        _stateMatrix[state, nextState](character, context);
                        state = nextState;
                        break;

                    case 5:
                        if (Numbers.Contains(character))
                            nextState = 5;
                        else if (character == ']')
                            nextState = 6;
                        else
                            throw new Exception($"Invalid character '{character}' in collection index at position {i}");

                        _stateMatrix[state, nextState](character, context);
                        state = nextState;
                        break;

                    case 6:
                        if (character == '}')
                            nextState = 1;
                        else
                            throw new Exception($"Expected '}}' but found {character} at position {i}");

                        _stateMatrix[state, nextState](character, context);
                        state = nextState;
                        break;
                }
            }

            //if (context.OpenCurlyBraceCount != context.CloseCurlyBraceCount)
            //{
            //    throw new Exception("Amount of opening curly braces does not match amount of closing");
            //}

            if (state != 1)
            {
                throw new Exception("The string does not match requirements");
            }

            return context.ResultBuilder.ToString();
        }

        private static void ErrorState(char character, ParserContext context)
        {
            throw new Exception("Invalid string format");
        }

        private static void State11(char character, ParserContext context)
        {
            context.ResultBuilder.Append(character);
        }

        private static void State12(char character, ParserContext context)
        {
            //context.OpenCurlyBraceCount++;
        }

        private static void State13(char character, ParserContext context)
        {
            //context.CloseCurlyBraceCount++;
        }

        private static void State21(char character, ParserContext context)
        {
            context.ResultBuilder.Append(character);
            //context.OpenCurlyBraceCount--;
        }

        private static void State24(char character, ParserContext context)
        {
            context.ExpressionBuilder.Clear();
            context.ExpressionBuilder.Append(character);
        }

        private static void State31(char character, ParserContext context)
        {
            context.ResultBuilder.Append(character);
        }

        private static void State41(char character, ParserContext context)
        {
            // add identifier to cash
            // add value of field to result
            context.ResultBuilder.Append(context.ExpressionBuilder.ToString());
            //context.CloseCurlyBraceCount++;
        }

        private static void State44(char character, ParserContext context)
        {
            context.ExpressionBuilder.Append(character);
        }
        
        private static void State45(char character, ParserContext context)
        {
            context.ExpressionBuilder.Append(character);
        }

        private static void State55(char character, ParserContext context)
        {
            context.ExpressionBuilder.Append(character);
        }

        private static void State56(char character, ParserContext context)
        {
            context.ExpressionBuilder.Append(character);
        }

        private static void State61(char character, ParserContext context)
        {
            // add to cash
            context.ResultBuilder.Append(context.ExpressionBuilder.ToString());
        }
    }
}
