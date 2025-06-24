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
        public Action<object> stdout;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            global_contracts.Clear();
        }

        //----------------------------------------------------------------------------------------------------------

        public static void AddContract(in Contract contract, params string[] aliases)
        {
            global_contracts.Add(contract.name, contract);
            for (int i = 0; i < aliases.Length; i++)
                global_contracts.Add(aliases[i], contract);
        }

        //----------------------------------------------------------------------------------------------------------

        public Harbinger(in Shell shell, in Harbinger father, in string workdir, in Action<object> stdout)
        {
            this.shell = shell;
            this.father = father;
            if (shell != null)
                this.stdout += data => this.shell.AddLine(data);
            this.stdout += stdout;
            this.workdir = PathCheck(workdir, PathModes.ForceFull, false, false, out _, out _);
        }

        //----------------------------------------------------------------------------------------------------------

        public void Stderr(string error)
        {
            error += "\n\n" + Util.GetStackTrace().GetFrame(1).ToString();
            _stderr ??= error;
            Debug.LogWarning(error);
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