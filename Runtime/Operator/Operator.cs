using _ARK_;
using System;
using System.Collections.Generic;

namespace _BOA_
{
    public sealed partial class Operator : IDisposable
    {
        static readonly BoaSignal sig_tick = new(SIG_FLAGS_new.TICK, null);

        readonly Executor program;
        public readonly IEnumerator<Contract.Status> execution;

        public bool _disposed;

        //----------------------------------------------------------------------------------------------------------

        public Operator(in Executor program)
        {
            this.program = program;
            execution = program.EExecute();
            NUCLEOR.delegates.shell_tick += Tick;
        }

        //----------------------------------------------------------------------------------------------------------

        public void Tick() => PropagateSignal(sig_tick);
        public void PropagateSignal(in BoaSignal signal)
        {
            program.harbinger.signal = signal;
            if (!execution.MoveNext())
                Dispose();
        }

        //----------------------------------------------------------------------------------------------------------

        public void Dispose()
        {
            NUCLEOR.delegates.shell_tick -= Tick;

            if (_disposed)
                return;
            _disposed = true;

            execution.Dispose();
        }
    }
}