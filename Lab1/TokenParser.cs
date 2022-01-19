using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Lab1
{

    public class TokenParser
    {
        public readonly Dictionary<string, (TokenType tokenType, int priority)> StringTokens;

        public int TokenMaxLength => StringTokens.Max(x => x.Key.Length);

        public string Expression { get; }

        public TokenParser(string expression)
        {
            Expression = expression;

            StringTokens = new Dictionary<string, (TokenType, int)>
            {
                { "(", (TokenType.LeftBracket, 0) },
                { ")", (TokenType.RightBracket, 0) },
                { "-", (TokenType.BinaryOperator, 2) },
                { "+", (TokenType.BinaryOperator, 2) },
                { "*", (TokenType.BinaryOperator, 1) },
                { "/", (TokenType.BinaryOperator, 1) },
                { "^", (TokenType.BinaryOperator, 1) },
                { ",", (TokenType.ArgumentSeparator, -1) },
                { ".", (TokenType.NumberSeparator, -1) },
                { "sin", (TokenType.Function, 1) },
                { "cos", (TokenType.Function, 1) },
                { "log", (TokenType.Function, 1) },
                { "pow", (TokenType.Function, 1) }
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
                    tokenType.tokenType != TokenType.NumberSeparator)
                {
                    compoundSymbol.Clear();
                    yield return new Token(tokenType.tokenType, tokenType.priority, compoundString);
                    continue;
                }

                if (TryGetNumber(compoundString, out var number) &&
                    (i == Expression.Length - 1 ||
                     i < Expression.Length - 1 &&
                     Expression[i + 1] != '.' &&
                     !IsNumber(Expression[i + 1].ToString())))
                {
                    compoundSymbol.Clear();
                    yield return new Token(TokenType.Number, -1, number);
                    continue;
                }

                if (compoundString.Length >= TokenMaxLength &&
                    !IsNumber(compoundString))
                {
                    compoundSymbol.Clear();
                    yield return new Token(TokenType.Invalid, -1, compoundString);
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