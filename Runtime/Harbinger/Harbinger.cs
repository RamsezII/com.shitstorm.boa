using System;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    public partial class Harbinger
    {
        static readonly Dictionary<string, Contract> global_contracts = new(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<string, Variable<object>> global_variables = new(StringComparer.Ordinal);

        public readonly Action<object> stdout;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            global_contracts.Clear();
        }

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            InitContracts();
            InitCmd_Run();
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
    }
}