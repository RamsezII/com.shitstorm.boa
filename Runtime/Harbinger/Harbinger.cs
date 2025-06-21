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

        public readonly Shell shell;
        public readonly Harbinger father;
        public BoaSignal signal;
        public readonly List<object> args = new();
        public string _stderr;
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

        public Harbinger(in Shell shell, in Harbinger father, in Action<object> stdout)
        {
            this.shell = shell;
            this.father = father;
            if (shell != null)
                this.stdout += data => this.shell.AddLine(data);
            this.stdout += stdout;
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

        public void Stderr(in string error)
        {
            _stderr ??= error;
            Debug.LogWarning(_stderr);
        }

        public bool TryPullError(out string error)
        {
            if (_stderr == null)
            {
                error = null;
                return false;
            }

            error = _stderr;
            _stderr = null;
            return true;
        }
    }
}