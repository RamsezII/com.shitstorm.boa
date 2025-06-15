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
                    if (!exe.reader.TryReadArgument(out string varname, as_function_argument: false))
                        exe.error ??= $"Expected variable name after 'var'.";
                    if (exe.pipe_previous == null && !exe.reader.TryReadChar_match('='))
                        exe.error ??= $"Expected '=' after variable name '{varname}'.";
                    else if (exe.pipe_previous == null && !exe.harbinger.TryParseExpression(exe.reader, exe, false, out expr))
                        exe.error ??= exe.reader.error ?? $"Failed to parse expression after '=' for variable '{varname}'.";
                    else
                    {
                        BoaVariable variable = new(null);
                        exe.caller._variables.Add(varname, variable);
                        exe.arg_0 = expr;
                        exe.args.Add(variable);
                    }
                },
                routine: static exe =>
                {
                    BoaVariable variable = (BoaVariable)exe.args[0];
                    return Executor.EExecute(
                        null,
                        data => variable.value = data,
                        exe.arg_0.EExecute());
                }));
        }
    }
}