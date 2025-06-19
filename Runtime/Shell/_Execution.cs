using UnityEngine;

namespace _BOA_
{
    public sealed partial class HarbingerShell : MonoBehaviour
    {
        static readonly BoaSignal sig_tick = new(SIG_FLAGS_new.TICK, null);
        internal BoaSignal signal;

        //----------------------------------------------------------------------------------------------------------

        void Tick() => PropagateSignal(sig_tick);
        void PropagateSignal(in BoaSignal signal)
        {
            this.signal = signal;
        }
    }
}