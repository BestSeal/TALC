using System.Collections.Generic;

namespace Lab1
{
    public class TokensList
    {
        public readonly Dictionary<char, TokenType> CharTokens;
        public readonly Dictionary<string, TokenType> StringTokens;

        public TokensList()
        {
            StringTokens = new Dictionary<string, TokenType>();
            CharTokens = new Dictionary<char, TokenType>();
            CharTokens.Add('(', TokenType.LeftBracket);
            CharTokens.Add(')', TokenType.RightBracket);
            CharTokens.Add('-', TokenType.BinaryAndUnaryOperator);
            CharTokens.Add('+', TokenType.BinaryAndUnaryOperator);
            CharTokens.Add('*', TokenType.BinaryOperator);
            CharTokens.Add('/', TokenType.BinaryOperator);
            CharTokens.Add('^', TokenType.BinaryOperator);
            CharTokens.Add('!', TokenType.UnaryOperator);
            CharTokens.Add(',', TokenType.ArgumentSeparator);
            CharTokens.Add('.', TokenType.NumberSeparator);
            
            // Functions
            StringTokens.Add("sin", TokenType.Function);
            StringTokens.Add("cos", TokenType.Function);
            StringTokens.Add("log", TokenType.Function);
            StringTokens.Add("pow", TokenType.Function);
        }
    }
}