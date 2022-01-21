using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab3.StackAutomata
{
    public class Command
    {
        public readonly Symbol _readSymbol;

        public readonly Symbol _popStackSymbol;

        public readonly List<Symbol> _pushStackExpression;

        public bool WasExecuted { get; set; }

        public bool ExecutionResult { get; set; }

        public string StackSnapshot { get; set; }

        public Stack<Symbol> ResultStack { get; set; }

        public string ExpressionSnapshot { get; set; }

        public bool IsFinalState => ResultStack?.Count == 0 && string.IsNullOrEmpty(ExpressionSnapshot);

        public Command(Symbol readSymbol, Symbol popStackSymbol, List<Symbol> pushStackExpression)
        {
            _readSymbol = readSymbol;
            _popStackSymbol = popStackSymbol;
            _pushStackExpression = pushStackExpression;
        }

        public bool IsExecutable(Stack<Symbol> stack, string expression) =>
            (_readSymbol.Value == Symbol.EpsilonSymbol &&
             _popStackSymbol.IsStackButtonSymbol) ||
            (!string.IsNullOrEmpty(expression) &&
             expression[0] == _readSymbol.Value ||
             _readSymbol.Value == Symbol.EpsilonSymbol) &&
            stack.TryPeek(out var peeked) &&
            peeked.Value == _popStackSymbol.Value;

        public (string expression, Stack<Symbol> stack, bool isSuccess) Execute(Stack<Symbol> stack, string expression)
        {
            var tmpStack = new Stack<Symbol>(stack?.Reverse() ?? new Stack<Symbol>());

            var isSuccess = true;
            WasExecuted = true;

            if (!_readSymbol.IsEpsilonSymbol)
            {
                if (expression[0] != _readSymbol.Value)
                {
                    isSuccess = false;
                }
                else
                {
                    expression = expression.Substring(1);
                }
            }

            if (!tmpStack.TryPeek(out var peeked) || peeked?.Value != _popStackSymbol.Value)
            {
                isSuccess = false;
            }
            else
            {
                if (tmpStack.Count > 0)
                {
                    tmpStack.Pop();
                }
                else
                {
                    isSuccess = false;
                }

                if (_pushStackExpression?.Any() == true && _pushStackExpression[0].Value != Symbol.EpsilonSymbol)
                {
                    foreach (var symbol in _pushStackExpression)
                    {
                        tmpStack.Push(symbol);
                    }
                }
            }

            StackSnapshot = string.Join("", tmpStack.Select(x => x.Value).Reverse().ToList());
            ExpressionSnapshot = expression;
            ExecutionResult = isSuccess;
            ResultStack = tmpStack;

            return (expression, tmpStack, isSuccess);
        }

        public override string ToString()
        {
            return
                $"({_readSymbol.Value}, {_popStackSymbol.Value}, {string.Join("", _pushStackExpression.Select(x => x.Value))})";
        }

        public override bool Equals(object? obj)
        {
            var res = obj is Command command &&
                      command._readSymbol?.Value == _readSymbol?.Value &&
                      command._popStackSymbol?.Value == _popStackSymbol?.Value &&
                      SymbolsCompare(command._pushStackExpression, _pushStackExpression);

            return res;
        }

        public static bool SymbolsCompare(List<Symbol> s1, List<Symbol> s2)
        {
            if (s1.Count != s2.Count) return false;

            for (int i = 0; i < s1.Count; i++)
            {
                if (s1[i]?.Value != s2[i]?.Value) return false;
            }

            return true;
        }
    }
}