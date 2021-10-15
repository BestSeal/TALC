using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab1
{
    public class TokenParser
    {
        public readonly Dictionary<string, TokenType> StringTokens;

        public string Expression { get; }
        
        public TokenParser(string expression)
        {
            Expression = expression;
            
            StringTokens = new Dictionary<string, TokenType>
            {
                { "(", TokenType.LeftBracket },
                { ")", TokenType.RightBracket },
                { "-", TokenType.BinaryAndUnaryOperator },
                { "+", TokenType.BinaryAndUnaryOperator },
                { "*", TokenType.BinaryOperator },
                { "/", TokenType.BinaryOperator },
                { "^", TokenType.BinaryOperator },
                { "!", TokenType.UnaryOperator },
                { ",", TokenType.ArgumentSeparator },
                { "sin", TokenType.Function },
                { "cos", TokenType.Function },
                { "log", TokenType.Function },
                { "pow", TokenType.Function }
            };
        }

        public IEnumerable<Token> GetNextToken()
        {
            var tempStringBuilder = new StringBuilder();
            TokenType prevTokenType = TokenType.Invalid;
            double? tempDoubleValue = null;
            int? tempIntValue = null;
            string? tempStringValue = null;
            
            var tokenType = StringTokens.TryGetValue(Expression[0].ToString(), out var type) ? type : TokenType.Invalid;
            
            foreach (var stringSymbol in Expression.Remove(0,1).Select(symbol => symbol.ToString()))
            {
                if (tokenType == TokenType.Invalid)
                {
                    tempStringBuilder.Append(stringSymbol);

                    tempStringValue = tempStringBuilder.ToString();
                    
                    if (StringTokens.TryGetValue(tempStringValue, out type))
                    {
                        tokenType = type;
                        tempDoubleValue = null;
                        tempIntValue = null;
                        tempStringBuilder.Append(stringSymbol);
                    }
                    else if (int.TryParse(tempStringValue, out int intValue))
                    {
                        tokenType = TokenType.Number;
                        tempIntValue = intValue;
                        tempStringBuilder.Append(stringSymbol);
                    }
                    else if (double.TryParse(tempStringValue, out double doubleValue))
                    {
                        tokenType = TokenType.Number;
                        tempDoubleValue = doubleValue;
                        tempStringBuilder.Append(stringSymbol);
                    }

                    prevTokenType = tokenType;
                    
                    continue;
                    
                }

                tempStringBuilder.Clear();
                yield return new Token(prevTokenType, tempStringBuilder.ToString(), tempDoubleValue, tempIntValue);
            }
        }
    }
}