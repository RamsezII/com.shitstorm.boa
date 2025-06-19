using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    public sealed partial class Operator
    {
        static readonly BoaSignal sig_tick = new(SIG_FLAGS_new.TICK, null);
        internal BoaSignal signal;

        readonly Janitor janitor = new();
        readonly Harbinger harbinger;
        readonly IEnumerator<Contract.Status> execution;

        //----------------------------------------------------------------------------------------------------------

        public Operator(in Harbinger harbinger, in Executor program)
        {

        }

        //----------------------------------------------------------------------------------------------------------

        public void Tick()
        {
            PropagateSignal(sig_tick);
        }

        public void PropagateSignal(in BoaSignal signal)
        {
            harbinger.signal = signal;
            execution?.MoveNext();
        }
    }
}