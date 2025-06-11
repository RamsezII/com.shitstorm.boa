using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_Var()
        {
            AddContract(new("var",
                expects_parenthesis: false,
                args: static exe =>
                {
                    if (exe.reader.TryReadArgument(out string varname, out exe.error, as_function_argument: false))
                        if (exe.reader.HasNext())
                            if (exe.reader.TryReadChar('='))
                                if (exe.harbinger.TryParseExpression(exe.reader, false, out var expression, out exe.error))
                                {
                                    BoaVar variable = new(varname, null);
                                    exe.harbinger.global_variables[varname] = variable;
                                    exe.args.Add(expression);
                                    exe.args.Add(variable);
                                }
                },
                routine: static exe =>
                {
                    Executor expression = (Executor)exe.args[0];
                    BoaVar variable = (BoaVar)exe.args[1];
                    return Executor.EExecute(modify_output: data => variable.value = data, expression.EExecute());
                }));
        }
    }
}