using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    public sealed partial class Shell : MonoBehaviour
    {
        static readonly BoaSignal sig_tick = new(SIG_FLAGS_new.TICK, null);
        internal BoaSignal signal;

        readonly Janitor janitor = new();
        IEnumerator<Contract.Status> execution;

        //----------------------------------------------------------------------------------------------------------

        void Tick()
        {
            PropagateSignal(sig_tick);
        }

        void PropagateSignal(in BoaSignal signal)
        {
            this.signal = signal;
        }
    }
}