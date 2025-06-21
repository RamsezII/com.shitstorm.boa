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
                    if (exe.harbinger.TryParseVariable(exe.reader, exe.scope, out var var_exe))
                        if (exe.reader.TryReadArgument(out string operator_name, lint: exe.reader.lint_theme.operators, as_function_argument: false))
                            if (!Enum.TryParse(operator_name, true, out OperatorsM code))
                                exe.reader.sig_error ??= $"unknown operator '{operator_name}'";
                            else
                            {
                                if (exe.pipe_previous == null && exe.harbinger.TryParseExpression(exe.reader, exe.scope, false, out var expression))
                                    exe.arg_0 = expression;
                                else
                                    exe.reader.sig_error ??= $"assignation expect an expression";

                                exe.args.Add(code);
                                exe.args.Add(var_exe);
                            }
                },
                routine: static exe =>
                {
                    OperatorsM code = (OperatorsM)exe.args[0];
                    VariableExecutor var_exe = (VariableExecutor)exe.args[1];
                    var var_routine = var_exe.EExecute();

                    return Executor.EExecute(
                        after_execution: null,
                        modify_output: data =>
                        {
                            object value = var_routine.Current.output;
                            if (exe.scope.TryGetVariable(var_exe.var_name, out var variable))
                                return variable.value = (code & ~OperatorsM.assign) switch
                                {
                                    OperatorsM.add => (int)value + (int)data,
                                    OperatorsM.sub => (int)value - (int)data,
                                    OperatorsM.mul => (int)value * (int)data,
                                    OperatorsM.div => (int)value / (int)data,
                                    OperatorsM.div_int => (int)value / (int)data,
                                    OperatorsM.mod => (int)value % (int)data,
                                    OperatorsM.not => !(bool)data,
                                    OperatorsM.eq => Util.Equals2(value, data),
                                    OperatorsM.neq => !Util.Equals2(value, data),
                                    OperatorsM.gt => (int)value > (int)data,
                                    OperatorsM.lt => (int)value < (int)data,
                                    OperatorsM.ge => (int)value >= (int)data,
                                    OperatorsM.le => (int)value <= (int)data,
                                    OperatorsM.and => (bool)value && (bool)data,
                                    OperatorsM.or => (bool)value || (bool)data,
                                    OperatorsM.xor => (bool)value != (bool)data,
                                    _ => data,
                                };
                            exe.harbinger.Stderr($"could not find variable '{var_exe.var_name}'.");
                            return null;
                        },
                        exe.arg_0.EExecute(),
                        var_routine
                    );
                }));
        }
    }
}