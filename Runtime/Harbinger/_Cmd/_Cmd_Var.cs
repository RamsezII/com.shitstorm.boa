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
                    if (!exe.reader.TryReadArgument(out string varname, out exe.error, as_function_argument: false))
                        exe.error ??= $"Expected variable name after 'var'.";
                    if (exe.pipe_previous == null && !exe.reader.TryReadChar_match('='))
                        exe.error ??= $"Expected '=' after variable name '{varname}'.";
                    else if (exe.pipe_previous == null && !exe.harbinger.TryParseExpression(exe.reader, exe, false, out expr, out exe.error))
                        exe.error ??= $"Failed to parse expression after '=' for variable '{varname}'.";
                    else
                    {
                        BoaVar variable = new(varname, null);
                        exe.caller._variables[varname] = variable;
                        exe.args.Add(expr);
                        exe.args.Add(variable);
                    }
                },
                routine: static exe =>
                {
                    Executor expression = (Executor)exe.args[0];
                    BoaVar variable = (BoaVar)exe.args[1];
                    return Executor.EExecute(null, data => variable.value = data, expression.EExecute());
                }));
        }
    }
}