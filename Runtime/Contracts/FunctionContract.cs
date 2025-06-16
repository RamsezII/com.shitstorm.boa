using System;
using System.Collections.Generic;

namespace _BOA_
{
    internal sealed class FunctionContract : Contract
    {
        readonly Executor caller;
        readonly string text;
        internal readonly List<string> args_names = new();

        //----------------------------------------------------------------------------------------------------------

        FunctionContract(in string name, in string text, in Executor caller,
            in int args_count,
            in bool function_style_arguments = true,
            in bool no_semicolon_required = false,
            in bool no_parenthesis = false,
            in Action<ContractExecutor> args = null,
            in Func<ContractExecutor, IEnumerator<Status>> routine = null
            )
            : base(name,
                  min_args: args_count,
                  max_args: args_count,
                  function_style_arguments: function_style_arguments,
                  no_semicolon_required: no_semicolon_required,
                  no_parenthesis: no_parenthesis,
                  args: args,
                  routine: routine
                  )
        {
            this.caller = caller;
            this.text = text;
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryParseFunction(BoaReader reader, Executor caller)
        {
            int read_old = reader.read_i;
            if (!reader.TryReadString_match("func", lint: reader.lint_theme.keywords))
            {
                reader.read_i = read_old;
                goto failure;
            }

            if (!reader.TryReadArgument(out string func_name, lint: reader.lint_theme.functions, as_function_argument: false))
            {
                reader.error ??= $"please specify a function name";
                goto failure;
            }

            bool expects_parenthesis = reader.strict_syntax;
            bool found_parenthesis = reader.TryReadChar_match('(', lint: reader.OpenBraquetLint());

            if (expects_parenthesis && !found_parenthesis)
            {
                reader.error ??= $"'{func_name}' expected opening parenthesis '('";
                goto failure;
            }

            List<string> args_names = new();
            while (reader.TryReadArgument(out string arg_name, lint: reader.lint_theme.variables, as_function_argument: true))
                args_names.Add(arg_name);

            if (reader.error != null)
                goto failure;

            if ((expects_parenthesis || found_parenthesis) && !reader.TryReadChar_match(')', lint: reader.CloseBraquetLint()))
            {
                reader.error ??= $"'{func_name}' expected closing parenthesis ')'";
                goto failure;
            }

            var func_exe = new FunctionExecutor(caller.harbinger, caller);

            for (int i = 0; i < args_names.Count; i++)
                func_exe._variables.Add(args_names[i], null);

            read_old = reader.read_i;
            if (!caller.harbinger.TryParseBlock(reader, func_exe, out _))
                goto failure;

            FunctionContract function = null;
            function = new FunctionContract(func_name,
                reader.text[read_old..reader.read_i],
                caller: caller,
                args_count: args_names.Count,
                args: exe =>
                {
                    for (int i = 0; i < args_names.Count; i++)
                        if (exe.harbinger.TryParseExpression(exe.reader, exe, true, out var expr))
                            exe._variables.Add(args_names[i], new BoaVariable(expr));

                    exe.caller = caller;

                    exe.args.Add(null);
                    var reader2 = BoaReader.ReadCommandLines(reader.lint_theme, reader.strict_syntax, command_lines: function.text.Split('\n', '\r', StringSplitOptions.None));

                    if (exe.harbinger.TryParseBlock(reader2, exe, out var block))
                        exe.args.Add(block);
                    else
                        exe.error ??= reader2.error;
                },
                routine: ERoutine);

            caller._functions.Add(func_name, function);

            return true;

        failure:
            return false;

            IEnumerator<Status> ERoutine(ContractExecutor exe)
            {
                for (int i = 0; i < args_names.Count; i++)
                {
                    string arg_name = args_names[i];
                    BoaVariable bvar = exe._variables.Get(arg_name);
                    ExpressionExecutor expr = (ExpressionExecutor)bvar.value;

                    var expr_routine = expr.EExecute();
                    while (expr_routine.MoveNext())
                        yield return expr_routine.Current;

                    bvar.value = expr_routine.Current.output;
                }

                Executor block = (Executor)exe.args[1];
                var block_routine = block.EExecute();
                while (block_routine.MoveNext())
                    yield return block_routine.Current;
            }
        }
    }
}