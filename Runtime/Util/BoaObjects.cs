using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        //----------------------------------------------------------------------------------------------------------

        public BoaVariable Dedoublate() => new(value);
    }

    public sealed class ScopeNode
    {
        static byte _id;
        public readonly byte id;
        public readonly ScopeNode parent;
        HashSet<ScopeNode> children;

        Dictionary<string, BoaVariable> _variables;
        Dictionary<string, FunctionContract> _functions;

        public override string ToString() => $"scope[{id}]";
#if UNITY_EDITOR
        string _tostring => ToString();
#endif

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            _id = 0;
        }

        //----------------------------------------------------------------------------------------------------------

        public ScopeNode(in ScopeNode parent)
        {
            id = ++_id;
            this.parent = parent;

            if (parent != null)
                (parent.children ??= new()).Add(this);
        }

        //----------------------------------------------------------------------------------------------------------

        public ScopeNode Dedoublate()
        {
            var clone = new ScopeNode(parent);

            if (_variables != null && _variables.Count > 0)
            {
                clone._variables = new(StringComparer.Ordinal);
                foreach (var pair in _variables)
                    clone._variables[pair.Key] = pair.Value?.Dedoublate();
            }

            if (_functions != null && _functions.Count > 0)
                clone._functions = new(_functions, StringComparer.Ordinal);

            if (children != null && children.Count > 0)
            {
                clone.children = new();
                foreach (ScopeNode child in children)
                    clone.children.Add(child.Dedoublate());
            }

            return clone;
        }

        internal void AddVariable(in string name, in BoaVariable value) => Add(ref _variables, name, value);
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

        internal void AddFunction(in string name, in FunctionContract value) => Add(ref _functions, name, value);
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

        static void Add<T>(ref Dictionary<string, T> _dict, in string name, in T value) where T : class => (_dict ??= new(StringComparer.Ordinal)).Add(name, value);
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