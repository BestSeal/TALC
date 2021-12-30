using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Lab1
{
    public class TokenParser
    {
        public readonly Dictionary<string, TokenType> StringTokens;

        public int TokenMaxLength => StringTokens.Max(x => x.Key.Length);

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
                { ".", TokenType.NumberSeparator },
                { "sin", TokenType.Function },
                { "cos", TokenType.Function },
                { "log", TokenType.Function },
                { "pow", TokenType.Function }
            };
        }

        public IEnumerable<Token> GetNextToken()
        {
            if (string.IsNullOrEmpty(Expression))
            {
                throw new ArgumentException($"{nameof(Expression)} can not be null or empty string");
            }

            var compoundSymbol = new StringBuilder(Expression.Length - 1);

            for (int i = 0; i < Expression.Length; ++i)
            {
                var symbol = Expression[i].ToString();
                compoundSymbol.Append(symbol);
                var compoundString = compoundSymbol.ToString();
                
                if (StringTokens.TryGetValue(compoundString, out var tokenType) && 
                    tokenType != TokenType.NumberSeparator)
                {
                    compoundSymbol.Clear();
                    yield return new Token(tokenType, compoundString);
                    continue;
                }
                
                if (TryGetNumber(compoundString, out var number) &&
                    (i == Expression.Length - 1 ||
                    i < Expression.Length - 1 &&
                    Expression[i + 1] != '.' &&
                    !IsNumber(Expression[i + 1].ToString())))
                {
                    compoundSymbol.Clear();
                    yield return new Token(TokenType.Number, number);
                    continue;
                }

                if (compoundString.Length >= TokenMaxLength && 
                    !IsNumber(compoundString))
                {
                    compoundSymbol.Clear();
                    yield return new Token(TokenType.Invalid, compoundString);
                }
            }
        }

        private static bool TryGetNumber(string str, out double? number)
        {
            if (double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var outNumber))
            {
                number = outNumber;
                return true;
            }

            number = null;
            return false;
        }

        private static bool IsNumber(string str)
            => double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var outNumber);
    }
}