using System;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    public abstract class Executor : IDisposable
    {
        sealed internal class BoaDict<T>
        {
            readonly BoaDict<T> parent;
            Dictionary<string, T> _variables;

            //----------------------------------------------------------------------------------------------------------

            internal BoaDict(in BoaDict<T> parent)
            {
                this.parent = parent;
            }

            //----------------------------------------------------------------------------------------------------------

            public void Add(in string name, in T value) => (_variables ??= new(StringComparer.Ordinal))[name] = value;
            public bool TryGet(string name, out T value)
            {
                if (_variables != null && _variables.TryGetValue(name, out value))
                    return true;
                else if (parent != null && parent != this && parent.TryGet(name, out value))
                    return true;
                value = default;
                return false;
            }
        }

        public readonly Harbinger harbinger;
        internal Executor caller;
        public string error;
        public bool disposed;

        static ushort _id;
        public readonly ushort id;
        public virtual string ToLog => $"{GetType().Name}[{id}]";

        internal readonly BoaDict<BoaVariable> _variables;
        internal readonly BoaDict<FunctionContract> _functions;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            _id = 0;
        }

        //----------------------------------------------------------------------------------------------------------

        protected Executor(in Harbinger harbinger, in Executor caller)
        {
            id = _id.LoopID();

            this.harbinger = harbinger;
            this.caller = caller;

            _variables = new(caller?._variables);
            _functions = new(caller?._functions);
        }

        //----------------------------------------------------------------------------------------------------------

        internal abstract IEnumerator<Contract.Status> EExecute();
        public static IEnumerator<Contract.Status> EExecute(Action<object> after_execution = null, Func<object, object> modify_output = null, params IEnumerator<Contract.Status>[] stack)
        {
            object data = null;

            if (stack != null && stack.Length > 0)
                for (int i = 0; i < stack.Length; i++)
                    if (stack[i] != null)
                    {
                        using var routine = stack[i];
                        while (routine.MoveNext())
                        {
                            data = routine.Current.data;
                            yield return routine.Current;
                        }
                    }

            after_execution?.Invoke(data);

            if (modify_output != null)
                yield return new Contract.Status(Contract.Status.States.ACTION_skip, data: modify_output(data));
        }

        //----------------------------------------------------------------------------------------------------------

        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            OnDispose();
        }

        protected virtual void OnDispose()
        {
        }
    }
}