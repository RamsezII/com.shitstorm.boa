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
            cmd_assign_ = AddContract(new("assign",
                args: static exe =>
                {
                    if (exe.reader.TryReadArgument(out string varname, out exe.error, as_function_argument: false))
                        if (exe.reader.TryReadArgument(out string operator_name, out exe.error, as_function_argument: false))
                            if (!Enum.TryParse(operator_name, true, out OperatorsM code))
                                exe.error = $"unknown operator '{operator_name}'";
                            else if (exe.harbinger.TryParseExpression(exe.reader, exe.parent, false, out var expression, out exe.error))
                            {
                                BoaVar variable = new(varname, null);
                                exe.harbinger.global_variables[varname] = variable;
                                exe.args.Add(code);
                                exe.args.Add(variable);
                                exe.args.Add(expression);
                            }
                },
                routine: static exe =>
                {
                    OperatorsM code = (OperatorsM)exe.args[0];
                    BoaVar variable = (BoaVar)exe.args[1];
                    Executor expr = (Executor)exe.args[2];

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
                    expr.EExecute());
                }));
        }
    }
}