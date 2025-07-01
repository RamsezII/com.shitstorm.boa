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

        static readonly Dictionary<string, Contract> global_contracts = new(StringComparer.OrdinalIgnoreCase);
        static readonly Dictionary<(Type type, string name), SubContract> sub_contracts = new();

        public readonly Shell shell;
        public readonly Harbinger father;
        public BoaSignal signal;
        public readonly List<object> args = new();
        public string _stderr;
        public Action<object, string> stdout;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            global_contracts.Clear();
            sub_contracts.Clear();
        }

        //----------------------------------------------------------------------------------------------------------

        public static void AddContract(in Contract contract, params string[] aliases)
        {
            global_contracts.Add(contract.name, contract);
            for (int i = 0; i < aliases.Length; i++)
                global_contracts.Add(aliases[i], contract);
        }

        public static void AddSubContract(in SubContract contract, params string[] aliases)
        {
            sub_contracts.Add((contract.input_type, contract.name), contract);
            for (int i = 0; i < aliases.Length; i++)
                sub_contracts.Add((contract.input_type, aliases[i]), contract);
        }

        public static bool TryGetSubContract(in Type type, string name, out SubContract subContract)
        {
            if (sub_contracts.TryGetValue((type, name), out subContract))
                return true;

            foreach (var pair in sub_contracts)
                if (pair.Key.name.Equals(name, StringComparison.Ordinal))
                    if (type.IsOfType(pair.Key.type))
                    {
                        subContract = pair.Value;
                        return true;
                    }

            return false;
        }

        //----------------------------------------------------------------------------------------------------------

        public Harbinger(in Shell shell, in Harbinger father, in string workdir, in Action<object, string> stdout)
        {
            this.shell = shell;
            this.father = father;
            if (shell != null)
                this.stdout += this.shell.AddLine;
            this.stdout += stdout;
            this.workdir = PathCheck(workdir, PathModes.ForceFull, false, false, out _, out _);
        }

        //----------------------------------------------------------------------------------------------------------

        public void Stderr(string error)
        {
            _stderr ??= error;
            error += "\n\n" + Util.GetStackTrace().GetFrame(1).ToString();
            Debug.LogWarning(error, shell);
        }
    }
}