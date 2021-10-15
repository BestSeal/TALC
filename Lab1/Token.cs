namespace Lab1
{
    public class Token
    {
        public TokenType TokenType { get; }

        public string? StringValue { get; }

        public double? DoubleValue { get; }

        public int? IntValue { get; }

        public Token(TokenType tokenType,
            string? stringValue = null,
            double? doubleValue = null,
            int? intValue = null)
        {
            TokenType = tokenType;
            StringValue = stringValue;
            DoubleValue = doubleValue;
            IntValue = intValue;
        }
    }
}