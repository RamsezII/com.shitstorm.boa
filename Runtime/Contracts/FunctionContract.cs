using System;
using System.Collections.Generic;

namespace _BOA_
{
    internal sealed class FunctionContract : Contract
    {
        FunctionContract(
            in string name,
            in int args_count,
            in Action<ContractExecutor> args = null,
            in Func<ContractExecutor, IEnumerator<Status>> routine = null
            )
            : base(name,
                  min_args: args_count,
                  max_args: args_count,
                  args: args,
                  routine: routine
                  )
        {
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryParseFunction(Harbinger harbinger, BoaReader reader, ScopeNode scope)
        {
            int read_old = reader.read_i;
            if (!reader.TryReadString_match("func", as_function_argument: false, lint: reader.lint_theme.keywords))
            {
                reader.read_i = read_old;
                goto failure;
            }

            if (!reader.TryReadArgument(out string func_name, lint: reader.lint_theme.functions, as_function_argument: false))
            {
                reader.error ??= $"please specify a function name";
                goto failure;
            }
            else if (scope.TryGetFunction(func_name, out _))
            {
                reader.error ??= $"function named '{func_name}' already defined in this scope";
                goto failure;
            }

            bool expects_parenthesis = reader.strict_syntax;
            bool found_parenthesis = reader.TryReadChar_match('(');

            if (found_parenthesis)
                reader.LintOpeningBraquet();

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

            var func_exe = new FunctionExecutor(harbinger, new ScopeNode(scope));

            for (int i = 0; i < args_names.Count; i++)
                func_exe.scope.AddVariable(args_names[i], null);

            read_old = reader.read_i;
            if (!harbinger.TryParseBlock(reader, func_exe.scope, out _))
                goto failure;
            string block_text = reader.text[read_old..reader.read_i];

            scope.AddFunction(
                func_name,
                new FunctionContract(
                    name: func_name,
                    args_count: args_names.Count,
                    args: exe =>
                    {
                        var func_scope = new ScopeNode(scope);

                        for (int i = 0; i < args_names.Count; i++)
                            if (exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out var expr))
                                func_scope.AddVariable(args_names[i], new BoaVariable(expr));
                            else
                                exe.error ??= $"could not parse expression for arg[{i}] '{args_names[i]}'";

                        exe.scope = func_scope;
                        var func_reader = BoaReader.ReadLines(reader.lint_theme, reader.strict_syntax, lines: block_text.Split('\n', '\r', StringSplitOptions.None));

                        if (exe.harbinger.TryParseBlock(func_reader, func_scope, out var func_block))
                            exe.args.Add(func_block);
                        else
                            exe.error ??= func_reader.error;
                    },
                    routine: ERoutine
                )
            );

            return true;

        failure:
            return false;

            IEnumerator<Status> ERoutine(ContractExecutor exe)
            {
                for (int i = 0; i < args_names.Count; i++)
                {
                    string arg_name = args_names[i];
                    BoaVariable bvar = exe.scope.GetVariable(arg_name);
                    ExpressionExecutor expr = (ExpressionExecutor)bvar.value;

                    var expr_routine = expr.EExecute();
                    while (expr_routine.MoveNext())
                        yield return expr_routine.Current;

                    bvar.value = expr_routine.Current.output;
                }

                Executor func_block = (Executor)exe.args[0];
                var block_routine = func_block.EExecute();
                while (block_routine.MoveNext())
                    yield return block_routine.Current;
            }
        }
    }
}