using System;
using System.Collections.Generic;
using System.Linq;

namespace _BOA_
{
    sealed internal class BoaDict<T>
    {
        readonly BoaDict<T> parent;
        Dictionary<string, T> _dict;

        //----------------------------------------------------------------------------------------------------------

        internal BoaDict(in BoaDict<T> parent)
        {
            this.parent = parent;
        }

        //----------------------------------------------------------------------------------------------------------

        public void Add(in string name, in T value) => (_dict ??= new(StringComparer.Ordinal))[name] = value;
        public T Get(in string name) => _dict[name];
        public bool TryGet(string name, out T value)
        {
            if (_dict != null && _dict.TryGetValue(name, out value))
                return true;
            else if (parent != null && parent != this && parent.TryGet(name, out value))
                return true;
            value = default;
            return false;
        }
    }

    internal class BoaVariable
    {
        public object value;

        //----------------------------------------------------------------------------------------------------------

        public BoaVariable(in object value)
        {
            this.value = value;
        }
    }

    public sealed class ScopeNode
    {
        public readonly ScopeNode parent;

        Dictionary<string, BoaVariable> _variables;
        Dictionary<string, FunctionContract> _functions;

        //----------------------------------------------------------------------------------------------------------

        public ScopeNode(in ScopeNode parent)
        {
            this.parent = parent;
        }

        //----------------------------------------------------------------------------------------------------------

        internal void SetVariable(in string name, in BoaVariable value) => Set(ref _variables, name, value);
        internal BoaVariable GetVariable(in string name) => Get(ref _variables, name);
        internal bool TryGetVariable(in string name, out BoaVariable value) => TryGet(ref _variables, name, out value) || parent != null && parent.TryGetVariable(name, out value);

        internal HashSet<KeyValuePair<string, BoaVariable>> GetVariables()
        {
            HashSet<KeyValuePair<string, BoaVariable>> set = new();
            GetVariables(set);
            return set;
        }
        void GetVariables(in HashSet<KeyValuePair<string, BoaVariable>> set)
        {
            if (_variables != null)
                set.UnionWith(_variables);
            parent?.GetVariables(set);
        }
        internal IEnumerable<string> EVarNames() => GetVariables().Select(v => v.Key);

        internal void SetFunction(in string name, in FunctionContract value) => Set(ref _functions, name, value);
        internal FunctionContract GetFunction(in string name) => Get(ref _functions, name);
        internal bool TryGetFunction(in string name, out FunctionContract value) => TryGet(ref _functions, name, out value) || parent != null && parent.TryGetFunction(name, out value);

        internal HashSet<KeyValuePair<string, FunctionContract>> GetFunctions()
        {
            HashSet<KeyValuePair<string, FunctionContract>> set = new();
            GetFunctions(set);
            return set;
        }
        void GetFunctions(in HashSet<KeyValuePair<string, FunctionContract>> set)
        {
            if (_functions != null)
                set.UnionWith(_functions);
            parent?.GetFunctions(set);
        }
        internal IEnumerable<string> EFuncNames() => GetFunctions().Select(f => f.Key);

        static void Set<T>(ref Dictionary<string, T> _dict, in string name, in T value) where T : class => (_dict ??= new(StringComparer.Ordinal))[name] = value;
        static T Get<T>(ref Dictionary<string, T> _dict, in string name) where T : class => _dict?[name];
        static bool TryGet<T>(ref Dictionary<string, T> _dict, string name, out T value) where T : class
        {
            if (_dict != null && _dict.TryGetValue(name, out value))
                return true;
            value = default;
            return false;
        }
    }
}