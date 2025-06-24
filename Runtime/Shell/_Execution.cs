using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class Shell
    {
        static readonly BoaSignal sig_tick = new(SIG_FLAGS_new.TICK, null);

        readonly ScopeNode scope = new(null, false);

        Harbinger harbinger;
        Executor program;
        IEnumerator<Contract.Status> execution;

        [SerializeField] Contract.Status shell_status;
        public Contract.Status current_status;

        const byte maximum_instant_ticks = 100;

        //----------------------------------------------------------------------------------------------------------

        void Tick() => PropagateSignal(sig_tick);
        public void PropagateSignal(in BoaSignal signal)
        {
            if (execution != null)
            {
                harbinger.signal = signal;

                int ticks = 0;
            before_tick:
                bool next = execution.MoveNext();
                workdir = harbinger.workdir;

                if (harbinger.TryPullError(out string error))
                {
                    AddLine(error, error.SetColor(Color.orange));
                    Clean();
                }
                else if (next)
                {
                    if (execution.Current.state == Contract.Status.States.ACTION_skip)
                        if (ticks++ < maximum_instant_ticks)
                            goto before_tick;
                        else
                            Debug.LogWarning($"{ToLog} reached the loop limit ({maximum_instant_ticks} ticks), waiting one frame.", this);
                    current_status = execution.Current;
                }
                else
                {
                    if (program.IsMarkedAsOutput())
                        AddLine(execution.Current.output);
                    Clean();
                    RefreshShellPrefixe();
                }

                void Clean()
                {
                    execution.Dispose();
                    execution = null;

                    program.Dispose();
                    program = null;

                    harbinger = null;
                }
            }
            else
            {
                current_status = shell_status;
                if (signal != null && signal.reader != null)
                {
                    bool submit = signal.flags.HasFlag(SIG_FLAGS_new.SUBMIT);

                    harbinger = new Harbinger(this, null, workdir, null);
                    harbinger.signal = signal;

                    var scope = this.scope;
                    if (!submit)
                        scope = scope.Dedoublate();

                    if (!harbinger.TryParseProgram(signal.reader, scope, out program))
                    {
                        if (submit)
                        {
                            signal.reader.LocalizeError();
                            string error = signal.reader.sig_long_error ?? signal.reader.sig_error;

                            if (error != null)
                            {
                                Debug.LogWarning(error, this);
                                AddLine(error, error.SetColor(Color.orange));
                            }
                        }
                        harbinger = null;
                    }
                    else if (submit)
                        execution = program.EExecute();
                    else
                        harbinger = null;
                }
            }
        }
    }
}