using System;
using System.Collections.Generic;

namespace _BOA_
{
    public class SubContract : Contract
    {
        public readonly Type object_type;
        public readonly Func<Type> get_object_type;

        //----------------------------------------------------------------------------------------------------------

        public SubContract(in string name, in Type object_type, in Type attribute_type,
            in Func<Type> get_object_type = null,
            in Func<ContractExecutor, Type> get_attribute_type = null,
            in int min_args = 0, in int max_args = 0,
            in bool function_style_arguments = true,
            in bool no_semicolon_required = false,
            in bool no_parenthesis = false,
            in bool outputs_if_end_of_instruction = false,
            in Action<ContractExecutor> opts = null,
            in Action<ContractExecutor> args = null,
            in Action<ContractExecutor> action = null,
            in Func<ContractExecutor, object> function = null,
            in Func<ContractExecutor, IEnumerator<Status>> routine = null
            ) :
            base(name, attribute_type,
                get_attribute_type,
                min_args, max_args,
                function_style_arguments,
                no_semicolon_required,
                no_parenthesis,
                outputs_if_end_of_instruction,
                opts, args, action, function, routine)
        {
            this.object_type = object_type;
            this.get_object_type = get_object_type;
        }
    }
}