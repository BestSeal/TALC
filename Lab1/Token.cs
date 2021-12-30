namespace Lab1
{
    public class Token
    {
        public TokenType TokenType { get; }

        public string? StringValue { get; }

        public double? DoubleValue { get; }

        public Token(TokenType tokenType,
            string? stringValue = null)
        {
            TokenType = tokenType;
            StringValue = stringValue;
        }

        public Token(TokenType tokenType,
            double? doubleValue = null)
        {
            TokenType = tokenType;
            DoubleValue = doubleValue;
            StringValue = doubleValue?.ToString();
        }

        public override string ToString()
            => $"{StringValue}[{TokenType}]";
    }
}