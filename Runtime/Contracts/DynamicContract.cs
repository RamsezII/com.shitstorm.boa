using System;
using System.Collections.Generic;

namespace _BOA_
{
    public class DynamicContract : Contract
    {
        internal readonly Executor parent;

        //----------------------------------------------------------------------------------------------------------

        DynamicContract(in string name, in Executor parent,
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
            this.parent = parent;
        }

        internal static bool TryParseFunction(in Harbinger harbinger, in Executor parent, in BoaReader reader, out string error)
        {
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

                List<string> args_names = new();

                while (reader.TryReadArgument(out string arg_name, out error, as_function_argument: true) && error == null)
                    args_names.Add(arg_name);

                if (error != null)
                    return false;

                if ((expects_parenthesis || found_parenthesis) && !reader.TryReadChar_match(')'))
                {
                    error ??= $"function declaration expected closing parenthesis ')'";
                    return false;
                }

                Executor function_body = null;
                var function = new DynamicContract(func_name, parent, args_names.Count, args_names.Count > 0 ? ReadArgs : null, ERoutine);

                var executor = new ContractExecutor(harbinger, parent, function, reader, parse_arguments: false);
                for (int i = 0; i < args_names.Count; ++i)
                {
                    string arg_name = args_names[i];
                    executor._variables[arg_name] = new BoaVar(arg_name, null);
                }

                if (!harbinger.TryParseBlock(reader, executor, out function_body, out error))
                {
                    error ??= $"could not parse body for function '{func_name}'";
                    return false;
                }

                parent._functions[func_name] = function;

                return true;

                void ReadArgs(ContractExecutor exe)
                {
                    for (int i = 0; i < args_names.Count; ++i)
                    {
                        string arg_name = args_names[i];
                        var variable = exe._variables[arg_name] = new(arg_name, null);

                        if (i == 0 && exe.pipe_previous != null)
                            exe.args.Add(null);

                        if (exe.harbinger.TryParseExpression(exe.reader, exe, true, out var expr, out exe.error))
                            exe.args.Add(expr);
                        else
                        {
                            exe.error ??= $"missing argument for function '{func_name}'";
                            return;
                        }
                    }
                }

                IEnumerator<Status> ERoutine(ContractExecutor exe)
                {
                    for (int i = 0; i < args_names.Count; ++i)
                    {
                        string arg_name = args_names[i];
                        ExpressionExecutor expr = (ExpressionExecutor)exe.args[i];

                        var expr_routine = expr.EExecute();
                        while (expr_routine.MoveNext())
                            yield return expr_routine.Current;

                        function_body._variables[arg_name] = new BoaVar(arg_name, expr_routine.Current.data);
                    }

                    var block_routine = function_body.EExecute();
                    while (block_routine.MoveNext())
                        yield return block_routine.Current;
                }
            }

            return false;
        }
    }
}