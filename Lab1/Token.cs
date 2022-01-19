using Lab1;

namespace Lab1
{

    public class Token
    {
        public TokenType TokenType { get; set; }

        public string? StringValue { get; }

        public double? DoubleValue { get; }

        public int TokenPriority { get; }

        public Token(TokenType tokenType, int tokenPriority,
            string? stringValue = null)
        {
            TokenType = tokenType;
            TokenPriority = tokenPriority;
            StringValue = stringValue;
        }

        public Token(TokenType tokenType, int tokenPriority,
            double? doubleValue = null)
        {
            TokenType = tokenType;
            TokenPriority = tokenPriority;
            DoubleValue = doubleValue;
            StringValue = doubleValue?.ToString();
        }

        public override string ToString()
            => $"{StringValue}[{TokenType}]";
    }
}