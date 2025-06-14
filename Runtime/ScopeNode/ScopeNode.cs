using System;
using System.Collections.Generic;

namespace _BOA_
{
    public class ScopeNode
    {
        public readonly ScopeNode parent;
        public readonly List<ScopeNode> children = new();
        public readonly Dictionary<string, BoaVar> _variables = new(StringComparer.Ordinal);
        public readonly Dictionary<string, FunctionContract> _functions = new(StringComparer.Ordinal);

        //----------------------------------------------------------------------------------------------------------

        public ScopeNode(in ScopeNode parent)
        {
            this.parent = parent;
            parent?.children.Add(this);
        }

        //----------------------------------------------------------------------------------------------------------

        public bool TryGetVariable(in string name, out BoaVar value)
        {
            if (_variables.TryGetValue(name, out value))
                return true;
            else if (parent != null && parent != this && parent.TryGetVariable(name, out value))
                return true;
            value = null;
            return false;
        }

        public bool TryGetFunction(in string name, out FunctionContract value)
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