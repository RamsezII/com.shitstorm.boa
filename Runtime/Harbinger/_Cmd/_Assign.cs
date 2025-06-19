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
                    if (exe.reader.TryReadArgument(out string var_name, lint: exe.reader.lint_theme.variables, as_function_argument: false))
                        if (exe.reader.TryReadArgument(out string operator_name, lint: exe.reader.lint_theme.operators, as_function_argument: false))
                            if (!Enum.TryParse(operator_name, true, out OperatorsM code))
                                exe.error ??= $"unknown operator '{operator_name}'";
                            else
                            {
                                if (exe.pipe_previous == null && exe.harbinger.TryParseExpression(exe.reader, exe.scope, false, out var expression))
                                    exe.arg_0 = expression;
                                else
                                    exe.error ??= $"assignation expect an expression";

                                exe.args.Add(code);
                                exe.args.Add(var_name);
                            }
                },
                routine: static exe =>
                {
                    OperatorsM code = (OperatorsM)exe.args[0];
                    string var_name = (string)exe.args[1];

                    return Executor.EExecute(
                        modify_output: data =>
                        {
                            if (!exe.scope.TryGetVariable(var_name, out var variable))
                            {
                                exe.error ??= $"could not find variable named '{var_name}'";
                                return null;
                            }
                            else
                                return variable.value = (code & ~OperatorsM.assign) switch
                                {
                                    OperatorsM.add => (int)variable.value + (int)data,
                                    OperatorsM.sub => (int)variable.value - (int)data,
                                    OperatorsM.mul => (int)variable.value * (int)data,
                                    OperatorsM.div => (int)variable.value / (int)data,
                                    OperatorsM.div_int => (int)variable.value / (int)data,
                                    OperatorsM.mod => (int)variable.value % (int)data,
                                    OperatorsM.not => !(bool)data,
                                    OperatorsM.eq => Util.Equals2(variable.value, data),
                                    OperatorsM.neq => !Util.Equals2(variable.value, data),
                                    OperatorsM.gt => (int)variable.value > (int)data,
                                    OperatorsM.lt => (int)variable.value < (int)data,
                                    OperatorsM.ge => (int)variable.value >= (int)data,
                                    OperatorsM.le => (int)variable.value <= (int)data,
                                    OperatorsM.and => (bool)variable.value && (bool)data,
                                    OperatorsM.or => (bool)variable.value || (bool)data,
                                    OperatorsM.xor => (bool)variable.value != (bool)data,
                                    _ => data,
                                };
                        },
                        stack: exe.arg_0.EExecute()
                    );
                }));
        }
    }
}