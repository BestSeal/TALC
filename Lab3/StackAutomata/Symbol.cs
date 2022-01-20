using System.Collections.Generic;

namespace Lab3.StackAutomata
{
    public class Symbol
    {
        public static string NonTerminalSymbols => "qwertyuiopasdfghjklzxcvbnm".ToUpperInvariant();

        public static char RulesSeparator => '|';

        public static char LeftSideSeparator => '>';

        public static char StackButtonSymbol = '_';

        public static char EpsilonSymbol = ' ';

        public readonly char Value;

        public bool IsTerminalSymbol => !NonTerminalSymbols.Contains(Value);

        public bool IsNotRuleSeparator => Value != RulesSeparator;

        public bool IsLeftSideSeparator => Value == LeftSideSeparator;

        public bool IsEpsilonSymbol => Value == EpsilonSymbol;

        public bool IsStackButtonSymbol => Value == StackButtonSymbol;

        public List<string> Rules;

        public Symbol(char value)
        {
            Value = value;
        }
    }
}