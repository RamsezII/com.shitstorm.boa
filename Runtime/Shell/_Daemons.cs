using _ARK_;
using System.Collections.Generic;

namespace _BOA_
{
    partial class Shell
    {
        public static void Daemonize(in Executor executor) => NUCLEOR.instance.subScheduler.AddRoutine(EDaemonize(executor));
        public static IEnumerator<float> EDaemonize(Executor executor)
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
    }
}