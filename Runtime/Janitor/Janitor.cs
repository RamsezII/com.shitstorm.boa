using _ARK_;
using System;
using System.Collections.Generic;

namespace _BOA_
{
    public sealed partial class Janitor : IDisposable
    {
        static readonly BoaSignal sig_tick = new(SIG_FLAGS_new.TICK, null);

        readonly Executor program;
        public readonly IEnumerator<Contract.Status> execution;
        public string error;
        public bool _disposed;

        //----------------------------------------------------------------------------------------------------------

        public Janitor(in Executor program)
        {
            this.program = program;
            program.janitor = this;
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