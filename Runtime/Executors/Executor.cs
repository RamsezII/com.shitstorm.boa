using System;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    public abstract class Executor : IDisposable
    {
        public readonly Harbinger harbinger;
        public Janitor janitor;
        public bool _disposed;

        static ushort _id;
        public readonly ushort id;
        public virtual string ToLog => $"{GetType().Name}[{id}]";

        public ScopeNode scope;

        internal bool is_instruction_output;
        public virtual Type OutputType() => null;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            _id = 0;
        }

        //----------------------------------------------------------------------------------------------------------

        protected Executor(in Harbinger harbinger, in ScopeNode scope)
        {
            id = _id++;
            this.harbinger = harbinger;
            this.scope = scope;
        }

        //----------------------------------------------------------------------------------------------------------

        internal virtual void MarkAsInstructionOutput() => is_instruction_output = true;
        internal virtual bool IsMarkedAsOutput() => is_instruction_output;

        public abstract IEnumerator<Contract.Status> EExecute();
        public static IEnumerator<Contract.Status> EExecute(Action<object> after_execution = null, Func<object, object> modify_output = null, params IEnumerator<Contract.Status>[] stack)
        {
            try
            {
                object data = null;

                if (stack != null && stack.Length > 0)
                    for (int i = 0; i < stack.Length; i++)
                        if (stack[i] != null)
                        {
                            using var routine = stack[i];
                            while (routine.MoveNext())
                            {
                                data = routine.Current.output;
                                yield return routine.Current;
                            }
                        }

                after_execution?.Invoke(data);

                if (modify_output != null)
                    yield return new Contract.Status(Contract.Status.States.ACTION_skip, output: modify_output(data));
            }
            finally
            {
                for (int i = 0; i < stack.Length; i++)
                    stack[i]?.Dispose();
            }
        }

        //----------------------------------------------------------------------------------------------------------

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            OnDispose();
        }

        protected virtual void OnDispose()
        {
        }
    }
}