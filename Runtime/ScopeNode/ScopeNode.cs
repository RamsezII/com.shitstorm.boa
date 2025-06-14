using System;
using System.Collections.Generic;

namespace _BOA_
{
    public class ScopeNode
    {
        public readonly ScopeNode parent;
        public readonly Dictionary<string, BoaVar> _variables = new(StringComparer.Ordinal);
        public readonly Dictionary<string, FunctionContract> _functions = new(StringComparer.Ordinal);

        //----------------------------------------------------------------------------------------------------------

        internal ScopeNode(in ScopeNode parent)
        {
            this.parent = parent;
        }

        //----------------------------------------------------------------------------------------------------------

        public bool TryGetVariable(string name, out BoaVar value)
        {
            if (_variables.TryGetValue(name, out value))
                return true;
            else if (parent != null && parent != this && parent.TryGetVariable(name, out value))
                return true;
            value = null;
            return false;
        }

        public bool TryGetFunction(string name, out FunctionContract value)
        {
            if (_functions.TryGetValue(name, out value))
                return true;
            else if (parent != null && parent != this && parent.TryGetFunction(name, out value))
                return true;
            value = null;
            return false;
        }
    }
}
