using System;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        static class CmdString
        {
            enum Operations
            {
                mirror,
            }

            //----------------------------------------------------------------------------------------------------------

            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
            static void Init_String()
            {
                AddContract(new("str",
                    min_args: 1,
                    args: static exe =>
                    {
                        ExpressionExecutor expr = null;
                        if (exe.pipe_previous == null && !exe.harbinger.TryParseExpression(exe.reader, exe, true, out expr, out exe.error))
                            exe.error ??= $"'{exe.contract.name}' expects an expression";
                        else if (!exe.reader.TryReadArgument(out string operation_name, out exe.error))
                            exe.error ??= "missing string operation";
                        else if (!Enum.TryParse(operation_name, true, out Operations op))
                            exe.error ??= $"unknown string operation '{operation_name}'";
                        else
                        {
                            exe.args.Add(expr);
                            exe.args.Add(op);
                        }
                    },
                    routine: static exe =>
                    {
                        Executor expression = (Executor)exe.args[0];
                        Operations op = (Operations)exe.args[1];

                        return Executor.EExecute(
                            null, data =>
                            {
                                if (data is string str)
                                    switch (op)
                                    {
                                        case Operations.mirror:
                                            return str.Mirror();
                                    }
                                exe.error = $"invalid data type '{data?.GetType()}' for string operation '{op}'";
                                return data;
                            },
                            expression.EExecute());
                    }));
            }
        }
    }
}