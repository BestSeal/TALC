using System;
using System.Collections.Generic;

namespace Lab2.Automata
{
    public class StateComparer : IEqualityComparer<State>
    {
        public bool Equals(State x, State y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Name == y.Name && x.NextStates.Equals(y.NextStates);
        }

        public int GetHashCode(State obj)
        {
            return HashCode.Combine(obj.Name, obj.NextStates);
        }
    }
}