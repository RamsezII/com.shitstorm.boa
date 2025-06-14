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

        public readonly Harbinger father;
        public readonly string script_path;
        public readonly bool strict_syntax;
        public readonly List<object> args = new();

        public string shell_stdin;
        public Action<object> stdout;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            global_contracts.Clear();
        }

        //----------------------------------------------------------------------------------------------------------

        public static void AddContract(in Contract contract) => global_contracts.Add(contract.name, contract);

        //----------------------------------------------------------------------------------------------------------

        public Harbinger(in Harbinger father, in Action<object> stdout, in string path, in bool strict_syntax = false)
        {
            this.father = father;
            this.stdout = stdout;
            script_path = path;
            this.strict_syntax = strict_syntax;
        }

        //----------------------------------------------------------------------------------------------------------

        public bool TryPullStdin(out string stdin)
        {
            if (Util.TryPullValue(ref shell_stdin, out stdin))
                return true;
            else if (father != null && father.TryPullStdin(out stdin))
                return true;
            return false;
        }
    }
}