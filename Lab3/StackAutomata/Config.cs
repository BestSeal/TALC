using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lab3.StackAutomata
{
    public class Config
    {
        public List<Symbol> StackSymbols { get; } = new();

        public List<Symbol> FiniteStates { get; } = new();

        public List<Command> Commands { get; private set; } = new();

        private Regex _lineValidationRegex = new(@".>(?:.*\|?)");

        public Config(string txtConfigPath)
        {
            if (File.Exists(txtConfigPath))
            {
                var lines = File.ReadAllLines(txtConfigPath);

                foreach (var line in lines)
                {
                    ParseConfigLine(line);
                }
            }

            CreateCommandsList();
        }

        private void ParseConfigLine(string line)
        {
            if (!_lineValidationRegex.IsMatch(line)) throw new AggregateException($"Invalid config line {line}");
            
            var nonTerminalSymbol = new Symbol(line[0]);

            if (StackSymbols.All(x => x.Value != nonTerminalSymbol.Value)) StackSymbols.Add(nonTerminalSymbol);

            for (int i = 2; i < line.Length; ++i)
            {
                var symbol = new Symbol(line[i]);
                if (symbol.IsNotRuleSeparator && StackSymbols.All(x => x.Value != symbol.Value))
                {
                    StackSymbols.Add(symbol);
                    if (symbol.IsTerminalSymbol) FiniteStates.Add(symbol);
                }
            }

            var rules = line.Substring(2).Split(Symbol.RulesSeparator).ToList();

            if (rules.Count == 0) throw new AggregateException($"No rules provided at line {line}");

            StackSymbols.Find(x => x.Value == nonTerminalSymbol.Value).Rules = rules;
        }

        public void CreateCommandsList()
        {
            var epsilonSymbol = new Symbol(Symbol.EpsilonSymbol);
            var stackButtonSymbol = new Symbol(Symbol.StackButtonSymbol);

            Commands = new List<Command>();

            // 1
            foreach (var symbol in StackSymbols.Where(s => !s.IsTerminalSymbol))
            {
                foreach (var rule in symbol.Rules)
                {
                    var pushStack = rule.Select(x => new Symbol(x)).Reverse().ToList();

                    Commands.Add(new Command(epsilonSymbol, symbol, pushStack));
                }
            }

            var emptyPushBack = new List<Symbol>().Append(epsilonSymbol).ToList();

            //2
            foreach (var symbol in StackSymbols.Where(s => s.IsTerminalSymbol))
            {
                Commands.Add(new Command(symbol, symbol, emptyPushBack));
            }

            //4
            Commands.Add(new Command(epsilonSymbol, stackButtonSymbol, emptyPushBack));
        }
    }
}