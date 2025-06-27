using _ARK_;
using System;
using System.Collections.Generic;

namespace _BOA_
{
    internal class Daemonizer : IDisposable
    {
        class Daemon : IDisposable
        {
            public readonly Executor executor;
            public readonly IEnumerator<Contract.Status> routine;

            public bool _disposed;

            static ushort _id;
            public readonly ushort id = _id++;

            //----------------------------------------------------------------------------------------------------------

            public Daemon(in Executor executor, in IEnumerator<Contract.Status> routine)
            {
                this.executor = executor;
                this.routine = routine;
            }

            //----------------------------------------------------------------------------------------------------------

            public void Dispose()
            {
                if (_disposed)
                    return;
                _disposed = true;
            }
        }

        static readonly Dictionary<ushort, Daemon> daemons = new();

        public bool _disposed;

        //----------------------------------------------------------------------------------------------------------

        internal void Init()
        {
            NUCLEOR.delegates.shell_tick += OnTick;
        }

        //----------------------------------------------------------------------------------------------------------

        void OnTick()
        {

        }

        public void Daemonize(in Executor executor)
        {
            EDaemonize(executor);
        }

        static IEnumerator<float> EDaemonize(Executor executor)
        {
            BoaSignal signal = new(SIG_FLAGS_new.TICK, null);
            using var routine = executor.EExecute();
            do
            {
                executor.harbinger.signal = signal;
                yield return routine.Current.progress;
            }
            while (routine.MoveNext());
        }

        //----------------------------------------------------------------------------------------------------------

        public void Dispose()
        {
            NUCLEOR.delegates.shell_tick -= OnTick;
            if (_disposed)
                return;
            _disposed = true;
        }
    }
}