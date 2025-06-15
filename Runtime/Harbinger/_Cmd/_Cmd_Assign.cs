using System;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        static Contract cmd_assign_;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_Assign()
        {
            AddContract(cmd_assign_ = new("assign",
                args: static exe =>
                {
                    if (exe.reader.TryReadArgument(out string varname, as_function_argument: false))
                        if (exe.reader.TryReadArgument(out string operator_name, as_function_argument: false))
                            if (!Enum.TryParse(operator_name, true, out OperatorsM code))
                                exe.error ??= $"unknown operator '{operator_name}'";
                            else
                            {
                                if (exe.pipe_previous == null && exe.harbinger.TryParseExpression(exe.reader, exe.caller, false, out var expression))
                                    exe.arg_0 = expression;
                                else
                                    exe.error ??= $"assignation expect an expression";

                                BoaVariable variable = new(null);
                                exe.caller._variables.Add(varname, variable);
                                exe.args.Add(code);
                                exe.args.Add(variable);
                            }
                },
                routine: static exe =>
                {
                    OperatorsM code = (OperatorsM)exe.args[0];
                    BoaVariable variable = (BoaVariable)exe.args[1];

                    return Executor.EExecute(null, data => variable.value = (code & ~OperatorsM.assign) switch
                    {
                        OperatorsM.add => (int)variable.value + (int)data,
                        OperatorsM.sub => (int)variable.value - (int)data,
                        OperatorsM.mul => (int)variable.value * (int)data,
                        OperatorsM.div => (int)variable.value / (int)data,
                        OperatorsM.div_int => (int)variable.value / (int)data,
                        OperatorsM.mod => (int)variable.value % (int)data,
                        _ => data,
                    },
                    exe.arg_0.EExecute());
                }));
        }
    }
}