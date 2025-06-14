using System;
using System.Collections.Generic;
using System.IO;
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

        public Action<object> stdout;

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

        public bool TryRunScript(in string path, out Executor executor, out string error, out string error_long, in bool strict_syntax = false)
        {
            BoaReader reader = new(strict_syntax, File.ReadAllText(path));

            if (!TryParseProgram(reader, out executor, out error) || error != null)
            {
                error_long = reader.LocalizeError(error, File.ReadAllLines(path));
                return false;
            }

            error_long = null;
            return true;
        }

        public bool TryParseProgram(in BoaReader reader, out Executor executor, out string error)
        {
            executor = null;

            BlockExecutor program = new(this, null);

            while (TryParseBlock(reader, program, out var sub_block, out error))
                if (sub_block != null)
                    program.stack.Add(sub_block);

            if (error != null)
                goto failure;

            if (reader.TryPeekChar_out(out char peek))
            {
                error ??= $"could not parse '{peek}'";
                goto failure;
            }

            executor = program;
            return true;

        failure:
            return false;
        }
    }
}