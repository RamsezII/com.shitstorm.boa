using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_Var()
        {
            AddContract(new("var",
                args: static exe =>
                {
                    if (exe.reader.TryReadArgument(out string varname))
                        if (exe.reader.HasNext())
                            if (exe.reader.TryReadChar('='))
                                if (exe.harbinger.TryParseExpression(exe.reader, out var expression, out exe.error))
                                {
                                    BoaVar variable = new(varname, null);
                                    exe.harbinger.global_variables[varname] = variable;
                                    exe.args.Add(variable);
                                    exe.args.Add(expression);
                                }
                },
                routine: static exe =>
                {
                    BoaVar variable = (BoaVar)exe.args[0];
                    Executor expression = (Executor)exe.args[1];
                    return Executor.EExecute(modify_output: data => variable.value = data, expression.EExecute());
                }));
        }
    }
}