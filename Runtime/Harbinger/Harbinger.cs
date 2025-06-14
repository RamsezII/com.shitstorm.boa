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
        readonly Dictionary<string, BoaVar> global_variables = new(StringComparer.Ordinal);

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

        public Executor ParseProgram(in BoaReader reader, out string error)
        {
            BlockExecutor program = new(this, null);

            while (TryParseBlock(reader, program, out var block, out error))
                if (block != null)
                    program.stack.Add(block);

            return program;
        }
    }
}