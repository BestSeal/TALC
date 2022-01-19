using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab2.Automata
{
    public class Automata
    {
        private Config Config { get; }
        public string ParsingErrors { get; }

        public bool IsInDfaForm =>
            States.All(x =>
                x.NextStates.All(ns =>
                    x.NextStates.Count(nsa =>
                        nsa.Symbol == ns.Symbol) == 1));

        public List<State> States { get; private set; } = new List<State>();
        
        public State CurrentState { get; private set; }

        public Automata(string automataConfig)
        {
            Config = new Config(automataConfig);
            try
            {
                States = Config.ParseTxtConfig();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ParsingErrors = e.Message;
            }
            
            CurrentState = States.FirstOrDefault();
        }

        public string GetAutomataInfo()
        {
            var infoBuilder = new StringBuilder();

            var hangedStates = States.Where(x =>
                States.All(y => y.Name == x.Name || y.NextStates.All(n => n.nextStateName != x.Name)) &&
                x.NextStates.All(xn => xn.nextStateName == x.Name));

            if (hangedStates.Any())
            {
                infoBuilder.Append("Automata has isolated states: ");
                infoBuilder.AppendLine(string.Join(", ", hangedStates.Select(x => $"{x.Name}")));
            }

            if (States.Count(x => x.IsFirstState) != 1)
            {
                infoBuilder.AppendLine("Automata doesn't have first state or has more than one");
            }

            if (!States.Any(x => x.IsFinalState))
            {
                infoBuilder.AppendLine("Automata doesn't have final states");
            }

            if (IsInDfaForm)
            {
                infoBuilder.AppendLine("Automata is in DFA form");
            }
            else
            {
                infoBuilder.AppendLine("Automata is in NFA form");
            }

            return infoBuilder.ToString();
        }

        public bool ParseExpression(string expression)
        {
            if (!IsInDfaForm)
            {
                ToDfa();
            }

            CurrentState = States.FirstOrDefault(x => x.IsFirstState);

            if (CurrentState == null)
            {
                throw new ArgumentException("First state not found");
            }
            
            if (!string.IsNullOrEmpty(expression))
            {
                foreach (char symbol in expression)
                {
                    var nextStateName = CurrentState.NextStates.FirstOrDefault(x => x.Symbol == symbol.ToString()).nextStateName;
                    
                    if (string.IsNullOrEmpty(nextStateName))
                    {
                        throw new ArgumentException($"Found impossible transition from {CurrentState.Name} by {symbol}. Not supported expression.");
                    }
                    
                    CurrentState = States.FirstOrDefault(x => x.Name == nextStateName);

                    if (CurrentState == null)
                    {
                        throw new AggregateException(
                            "Unexpected state name. Internal error has occured during expression parsing.");
                    }
                }

                if (CurrentState.IsFinalState)
                {
                    return true;
                }

                return false;
            }

            throw new ArgumentException("Empty expression passed");
        }

        public Automata ToDfa()
        {
            if (IsInDfaForm) return this;

            var deq = new Queue<State>();
            var resultStates = new List<State>();

            var state = States.FirstOrDefault(x => x.IsFirstState);

            if (state == null)
            {
                throw new ArgumentException("First state not found");
            }

            deq.Enqueue(state);

            while (deq.Count != 0)
            {
                state = deq.Dequeue();
                var grouped = state.NextStates.GroupBy(x => x.Symbol);
                var nextStates = new List<(string symbol, string nextState)>();

                foreach (var group in grouped)
                {
                    var symbol = group.Key;
                    var tmpNextStates = group
                        .Select(x => x.nextStateName)
                        .Distinct()
                        .Select(x => States.Find(s => s.Name == x) ?? resultStates.Find(s => s.Name == x))
                        .Select(x => (symbol, new State(x.Name)))
                        .ToList();

                    tmpNextStates = tmpNextStates.Where(x => tmpNextStates.Count(s => s.Item2.Name.Contains(x.Item2.Name)) == 1 ).ToList();
                    
                    var stateName = string.Join("", tmpNextStates.Union(tmpNextStates).Select(x => x.Item2.Name).ToList());
                    
                    if (stateName == state.Name)
                    {
                        nextStates.Add((symbol, stateName));
                        continue;
                    }
                    
                    var newState = new State(stateName);
                    newState.NextStates = States.Where(x => newState.Name.Contains(x.Name))
                        .SelectMany(x => x.NextStates).Distinct().ToList();
                    nextStates.Add((symbol, newState.Name));

                    if (deq.All(x => x.Name != newState.Name) && resultStates.All(x => x.Name != newState.Name))
                    {
                        deq.Enqueue(newState);
                    }
                }

                state.NextStates = nextStates;
                if (resultStates.All(x => x.Name != state.Name))
                {
                    resultStates.Add(state);
                }
            }

            States = resultStates;

            return this;
        }

        public string GetConfig()
        {
            var configBuilder = new StringBuilder("Automata config:\n");

            if (States?.Any() != true) return "States not found";
            
            foreach (var state in States)
            {
                foreach (var transition in state.NextStates)
                {
                    configBuilder.AppendLine($"{state.Name},{transition.Symbol}={transition.nextStateName}");
                }
            }

            return configBuilder.ToString();
        }
    }
}