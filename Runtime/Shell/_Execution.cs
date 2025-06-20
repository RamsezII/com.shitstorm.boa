using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class Shell
    {
        static readonly BoaSignal sig_tick = new(SIG_FLAGS_new.TICK, null);

        readonly ScopeNode scope = new(null);

        Harbinger harbinger;
        IEnumerator<Contract.Status> execution;
        readonly Janitor janitor = new();

        [SerializeField] Contract.Status shell_status;
        public Contract.Status current_status;

        //----------------------------------------------------------------------------------------------------------

        void Tick() => PropagateSignal(sig_tick);
        public void PropagateSignal(in BoaSignal signal)
        {
            if (execution != null)
            {
                harbinger.signal = signal;
                if (execution.MoveNext())
                {
                    current_status = execution.Current;
                    shell_status.prefixe = execution.Current.prefixe;
                }
                else
                {
                    harbinger = null;
                    execution = null;
                    shell_status.prefixe = GetPrefixe();
                    current_status = shell_status;
                }
            }
            else if (signal.reader != null)
            {
                bool submit = signal.flags.HasFlag(SIG_FLAGS_new.SUBMIT);

                harbinger = new Harbinger(this, null, null);
                harbinger.signal = signal;

                var scope = this.scope;
                if (!submit)
                    scope = scope.Dedoublate();

                harbinger.TryParseProgram(signal.reader, scope, out var program);

                if (submit)
                    execution = program.EExecute();
                else
                    harbinger = null;
            }
        }
    }
}