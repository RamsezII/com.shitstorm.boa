using System;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    public partial class Harbinger
    {
        /*

            Instruction
            │
            ├── Assignation (ex: x = ...)
            │     └── Expression
            │           └── ...
            │
            └── Expression
                └── Or
                    └── And
                        └── Comparison
                            └── Addition (addition, subtraction)
                                └── Term (multiplication, division, modulo)
                                    └── Facteur
                                        ├── Littéral (nombre)
                                        ├── Variable
                                        ├── Parenthèse
                                        └── Appel de fonction

        */

        internal static readonly Dictionary<string, Contract> global_contracts = new(StringComparer.OrdinalIgnoreCase);

        public readonly Action<object> stdout;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            global_contracts.Clear();
        }

        //----------------------------------------------------------------------------------------------------------

        public static Contract AddContract(in Contract contract)
        {
            global_contracts.Add(contract.name, contract);
            return contract;
        }

        //----------------------------------------------------------------------------------------------------------

        public Harbinger(in Action<object> stdout)
        {
            this.stdout = stdout;
        }

        //----------------------------------------------------------------------------------------------------------

        public bool TryParseProgram(in BoaReader reader, out Executor executor, out string error)
        {
            executor = null;
            BlockExecutor program = new(this, null);

            while (TryParseBlock(reader, program, out var sub_block, out error))
                if (sub_block != null)
                    program.stack.Add(sub_block);

            if (error != null)
                return false;

            if (reader.TryPeekChar_out(out char peek))
            {
                error ??= $"could not parse '{peek}'";
                return false;
            }

            executor = program;
            return true;
        }
    }
}