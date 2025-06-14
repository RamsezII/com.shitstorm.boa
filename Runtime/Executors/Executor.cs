using System;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    public abstract class Executor : IDisposable
    {
        public readonly Harbinger harbinger;
        internal Executor caller;
        public string error;
        public bool disposed;

        static ushort _id;
        public readonly ushort id;
        public virtual string ToLog => $"{GetType().Name}[{id}]";

        public readonly Dictionary<string, BoaVar> _variables = new(StringComparer.Ordinal);
        public readonly Dictionary<string, FunctionContract> _functions = new(StringComparer.Ordinal);

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            _id = 0;
        }

        //----------------------------------------------------------------------------------------------------------

        internal Executor(in Harbinger harbinger, in Executor caller)
        {
            id = _id.LoopID();
            this.harbinger = harbinger;
            this.caller = caller;
        }

        //----------------------------------------------------------------------------------------------------------

        public bool TryGetVariable(string name, out BoaVar value)
        {
            if (_variables.TryGetValue(name, out value))
                return true;
            else if (caller != null && caller != this && caller.TryGetVariable(name, out value))
                return true;
            value = null;
            return false;
        }

        public bool TryGetFunction(string name, out FunctionContract value)
        {
            if (_functions.TryGetValue(name, out value))
                return true;
            else if (caller != null && caller != this && caller.TryGetFunction(name, out value))
                return true;
            value = null;
            return false;
        }

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