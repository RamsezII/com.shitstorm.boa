using System;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    public static partial class Harbinger
    {
        static readonly Dictionary<string, Contract> global_contracts = new(StringComparer.OrdinalIgnoreCase);
        static readonly Dictionary<string, object> global_values = new(StringComparer.Ordinal);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            global_contracts.Clear();
            global_values.Clear();
        }

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            InitCmd_Run();
        }

        //----------------------------------------------------------------------------------------------------------

        public static Contract AddContract(in Contract contract)
        {
            global_contracts.Add(contract.name, contract);
            return contract;
        }
    }
}