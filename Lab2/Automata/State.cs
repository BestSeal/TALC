using System;
using System.Collections.Generic;

namespace Lab2.Automata
{
    public class State : IEquatable<State>
    {
        public string Name { get; }

        public bool IsFirstState => Name.Equals("q0") || Name.Equals("Q0");

        public bool IsFinalState => Name.Contains('f') || Name.Contains('F');

        public List<(string Symbol, string nextStateName)> NextStates { get; set; }

        public State(string name)
        {
            Name = name;
            NextStates = new List<(string Symbol, string nextStateName)>();
        }

        public void AddNextState(string symbol, State state) =>
            AddNextState((symbol, state.Name));

        public void AddNextState((string Symbol, string nextStateName) symbolStatePair) =>
            NextStates.Add(symbolStatePair);

        public bool Equals(State? other) => other?.Name.Contains(Name) == true;
    }
}