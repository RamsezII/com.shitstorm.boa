using System;
using System.Collections.Generic;

namespace _BOA_
{
    partial class Harbinger
    {
        static readonly Dictionary<string, Contract> global_contracts = new(StringComparer.OrdinalIgnoreCase);

        //----------------------------------------------------------------------------------------------------------

        public static Contract AddContract(in Contract contract)
        {
            global_contracts.Add(contract.name, contract);
            return contract;
        }
    }
}