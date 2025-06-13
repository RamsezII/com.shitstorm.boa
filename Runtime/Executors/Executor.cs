using System;
using System.Collections.Generic;

namespace _BOA_
{
    public abstract class Executor : IDisposable
    {
        public readonly Harbinger harbinger;
        public bool disposed;

        //----------------------------------------------------------------------------------------------------------

        public Executor(in Harbinger harbinger)
        {
            this.harbinger = harbinger;
        }

        //----------------------------------------------------------------------------------------------------------

        internal abstract IEnumerator<Contract.Status> EExecute(Action<object> after_execution = null);

        public static IEnumerator<Contract.Status> EExecute(Func<object, object> modify_output = null, params IEnumerator<Contract.Status>[] stack)
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

            if (modify_output != null)
                yield return new Contract.Status()
                {
                    state = Contract.Status.States.ACTION_skip,
                    data = modify_output?.Invoke(data),
                };
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