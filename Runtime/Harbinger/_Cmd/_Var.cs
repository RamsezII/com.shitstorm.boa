using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_Var()
        {
            AddContract(new("var",
                function_style_arguments: false,
                args: static exe =>
                {
                    ExpressionExecutor expr = null;
                    if (!exe.reader.TryReadArgument(out string varname, as_function_argument: false, lint: exe.reader.lint_theme.variables))
                        exe.error ??= $"Expected variable name after 'var'.";
                    if (exe.pipe_previous == null && !exe.reader.TryReadChar_match('=', lint: exe.reader.lint_theme.operators))
                        exe.error ??= $"Expected '=' after variable name '{varname}'.";
                    else if (exe.pipe_previous == null && !exe.harbinger.TryParseExpression(exe.reader, exe.scope, false, out expr))
                        exe.error ??= exe.reader.error ?? $"Failed to parse expression after '=' for variable '{varname}'.";
                    else
                    {
                        BoaVariable variable = new(null);
                        exe.scope.AddVariable(varname, variable);
                        exe.arg_0 = expr;
                        exe.args.Add(varname);
                    }
                },
                routine: static exe =>
                {
                    string var_name = (string)exe.args[0];
                    return Executor.EExecute(
                        after_execution: data => exe.scope.SetVariable(var_name, new(data)),
                        stack: exe.arg_0.EExecute());
                }));
        }
    }
}