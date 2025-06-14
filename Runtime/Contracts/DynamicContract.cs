using System;
using System.Collections.Generic;

namespace _BOA_
{
    internal class DynamicContract : Contract
    {
        DynamicContract(in string name,
            in int min_args, in int max_args,
            in bool function_style_arguments, in bool no_semicolon_required, in bool no_parenthesis,
            in Action<ContractExecutor> args, in Func<ContractExecutor, IEnumerator<Status>> routine)
            : base(name, min_args, max_args, function_style_arguments, no_semicolon_required, no_parenthesis, args, routine)
        {
        }

        public static DynamicContract Create(in Harbinger harbinger, in BoaReader reader)
        {
            return default;
        }
    }
}