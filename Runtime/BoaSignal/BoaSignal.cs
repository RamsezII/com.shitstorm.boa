using UnityEngine;

namespace _BOA_
{
    public sealed partial class BoaSignal
    {
        static byte _id;
        public readonly byte id;
        public readonly SIG_FLAGS_old flags;
        public readonly BoaReader reader;
        public override string ToString() => $"sig[{id}]{{{flags}}}";

#if UNITY_EDITOR
        string ToLog => ToString();
#endif

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            _id = 0;
        }

        //--------------------------------------------------------------------------------------------------------------

        public BoaSignal(in SIG_FLAGS_old flags, in BoaReader reader)
        {
            id = _id++;
            this.flags = flags;
            this.reader = reader;
        }
    }
}