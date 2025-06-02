using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        public abstract class AbstractContractor
        {
            public abstract IEnumerator<Contract.Status> EExecute();
        }

        public sealed class Contractor : AbstractContractor
        {
            static ushort _id;
            public readonly ushort id;

            public readonly Contract contract;
            public readonly List<object> args;
            public readonly IEnumerator<Contract.Status> routine;

            string text;
            int read_i;

            //----------------------------------------------------------------------------------------------------------

            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
            static void OnBeforeSceneLoad()
            {
                _id = 0;
            }

            //----------------------------------------------------------------------------------------------------------

            public Contractor(in Contract contract, in string text, ref int read_i)
            {
                id = _id.LoopID();

                this.contract = contract;

                if (contract.args != null)
                {
                    args = new();
                    this.text = text;
                    this.read_i = read_i;
                    contract.args(this);
                    this.text = null;
                    read_i = this.read_i;
                }

                if (contract.routine != null)
                    routine = contract.routine(this);
            }

            //----------------------------------------------------------------------------------------------------------

            public bool TryReadArgument(out string argument) => Util_boa.TryReadArgument(text, out _, ref read_i, out argument);

            //----------------------------------------------------------------------------------------------------------

            public override IEnumerator<Contract.Status> EExecute()
            {
                if (contract.action != null)
                    yield return contract.action(this);

                if (routine != null)
                    while (routine.MoveNext())
                        yield return routine.Current;
            }
        }

        public sealed class MegaContractor : AbstractContractor
        {
            public readonly List<AbstractContractor> stack = new();

            //----------------------------------------------------------------------------------------------------------

            public override IEnumerator<Contract.Status> EExecute()
            {
                int stack_i = 0;
                while (stack_i < stack.Count)
                {
                    var execution = stack[stack_i++].EExecute();
                    while (execution.MoveNext())
                        yield return execution.Current;
                }
            }
        }
    }
}