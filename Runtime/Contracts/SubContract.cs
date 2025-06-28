using System;
using System.Collections.Generic;

namespace _BOA_
{
    public class SubContract : Contract
    {
        public readonly Type input_type;
        public readonly Func<Type> get_input_type;

        //----------------------------------------------------------------------------------------------------------

        public SubContract(in string name,
            in Type input_type,
            in Type output_type,
            in Func<Type> get_input_type = null,
            in Func<ContractExecutor, Type> get_output_type = null,
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
            base(name,
                output_type: output_type,
                get_output_type: get_output_type,
                no_type_check: false,
                min_args: min_args,
                max_args: max_args,
                function_style_arguments: function_style_arguments,
                no_semicolon_required: no_semicolon_required,
                no_parenthesis: no_parenthesis,
                outputs_if_end_of_instruction: outputs_if_end_of_instruction,
                opts: opts,
                args: args,
                action: action,
                function: function,
                routine: routine)
        {
            this.input_type = input_type;
            this.get_input_type = get_input_type;
        }
    }
}