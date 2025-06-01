using System.Collections.Generic;

namespace _BOA_
{
    internal abstract class Instruction
    {
        public readonly IEnumerator<float> routine;
        protected virtual bool TryRun() => true;
        protected abstract IEnumerator<float> ERoutine();
    }
}