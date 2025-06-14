using System;
using System.Collections.Generic;

namespace _BOA_
{
    public class DynamicContract : Contract
    {
        DynamicContract(in string name,
            in int args_count,
            in Action<ContractExecutor> args,
            in Func<ContractExecutor, IEnumerator<Status>> routine
            )
            : base(name,
                  min_args: args_count,
                  max_args: args_count,
                  function_style_arguments: true,
                  no_semicolon_required: false,
                  no_parenthesis: false,
                  args: args,
                  routine: routine)
        {
        }

        internal static bool TryParseFunction(in Harbinger harbinger, in Executor parent, in BoaReader reader, out DynamicContract dynamic_contract, out string error)
        {
            dynamic_contract = null;
            error = null;
            int read_old = reader.read_i;

            if (!reader.TryReadString_match("func"))
            {
                reader.read_i = read_old;
                return false;
            }
            else if (!reader.TryReadArgument(out string func_name, out error))
                error ??= $"please specify a name for your function";
            else
            {
                bool expects_parenthesis = reader.strict_syntax;
                bool found_parenthesis = reader.TryReadChar_match('(');

                if (expects_parenthesis && !found_parenthesis)
                {
                    error ??= $"function declaration expected opening parenthesis '('";
                    return false;
                }

                List<string> args = new();

                while (reader.TryReadArgument(out string arg_name, out error, as_function_argument: true) && error == null)
                    args.Add(arg_name);

                if (error != null)
                    return false;

                if ((expects_parenthesis || found_parenthesis) && !reader.TryReadChar_match(')'))
                {
                    error ??= $"function declaration expected closing parenthesis ')'";
                    return false;
                }

                if (!harbinger.TryParseBlock(reader, parent, out var block, out error))
                {
                    error ??= $"could not parse body for function '{func_name}'";
                    return false;
                }

                dynamic_contract = new DynamicContract(func_name, args.Count, args.Count > 0 ? ReadArgs : null, exe => block.EExecute());
                return true;

                static void ReadArgs(ContractExecutor exe)
                {
                    for (int i = 0; i < exe.contract.max_args; ++i)
                        if (exe.reader.TryReadArgument(out string arg, out _))
                            exe.args.Add(arg);
                }
            }

            return false;
        }
    }
}