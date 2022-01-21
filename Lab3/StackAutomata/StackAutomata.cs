using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace Lab3.StackAutomata
{
    public class StackAutomata
    {
        private Stack<Symbol> SymbolsStack { get; set; } = new();

        private string _expression;

        private readonly Config _config;

        private List<List<Command>> CommandsOrder { get; set; } = new();

        public StackAutomata(Config config, string expression, string initStack)
        {
            _config = config;
            _expression = expression;
            SymbolsStack.Push(new Symbol(Symbol.StackButtonSymbol));

            if (!string.IsNullOrEmpty(initStack))
            {
                foreach (var symbol in initStack)
                {
                    SymbolsStack.Push(new Symbol(symbol));
                }
            }
        }

        public string GetExecutionOrder()
        {
            var finiteCommandsOrder = CommandsOrder.FirstOrDefault(x => x.Any(c => c.IsFinalState));

            if (finiteCommandsOrder?.Count > 0)
            {
                return string.Join(" |- ",
                    finiteCommandsOrder.Select(x => $"({x.ExpressionSnapshot},{x.StackSnapshot})").ToList());
            }

            return "No finite commands order was found";
        }

        public bool ParseExpression()
        {
            foreach (var command in _config.Commands)
            {
                if (command.IsExecutable(SymbolsStack, _expression))
                {
                    CommandsOrder.Add(new List<Command> { command });
                }
            }

            while (!CommandsOrder.Any(x => x.Any(s => s.IsFinalState)))
            {
                if (CommandsOrder.Count > 4000) return false;

                _config.CreateCommandsList();
                var updatedOrder = new List<List<Command>>();
                foreach (var commandsLine in CommandsOrder)
                {
                    var command = commandsLine.Last();

                    var (expr, stack, status) = (command.ExpressionSnapshot, command.ResultStack,
                        command.ExecutionResult);

                    if (!command.WasExecuted)
                    {
                        var exeStack = commandsLine.Count > 1
                            ? commandsLine[^2].ResultStack
                            : SymbolsStack;
                        var exeExpr = commandsLine.Count > 1
                            ? commandsLine[^2].ExpressionSnapshot
                            : _expression;
                        (expr, stack, status) = command.Execute(exeStack, exeExpr);
                    }

                    if (!status)
                    {
                        continue;
                    }

                    foreach (var commandBase in _config.Commands)
                    {
                        if (commandBase.IsExecutable(stack, expr) &&
                            // убрать повторные команды-циклы
                            !(commandBase._readSymbol.Value == Symbol.EpsilonSymbol &&
                              commandBase.Equals(command)))
                        {
                            updatedOrder.Add(commandsLine.Append(commandBase).ToList());
                        }
                    }
                }

                CommandsOrder.AddRange(updatedOrder);
                CommandsOrder = CommandsOrder
                    .Where(x => !x.Any(c => !c.ExecutionResult && c.WasExecuted))
                    .Distinct(new SymbolListsComparer())
                    .ToList();
                CommandsOrder = CommandsOrder.OrderByDescending(x => x.Count).ToList();
            }

            return true;
        }
    }

    public class SymbolListsComparer : IEqualityComparer<List<Command>>
    {
        public bool Equals(List<Command> x, List<Command> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            if (x.Count != y.Count) return false;

            for (int i = 0; i < x.Count; i++)
            {
                if (!x[i].Equals(y[i])) return false;
            }

            return true;
        }

        public int GetHashCode(List<Command> obj)
        {
            return HashCode.Combine(obj.Capacity, obj.Count);
        }
    }
}