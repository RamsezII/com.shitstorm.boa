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
                AddContract(new("string",
                    min_args: 1,
                    args: static exe =>
                    {
                        if (!exe.reader.TryReadArgument(out string operation_name))
                            exe.error = "missing string operation";
                        else if (!Enum.TryParse(operation_name, true, out Operations op))
                            exe.error = $"unknown string operation '{operation_name}'";
                        else if (!exe.harbinger.TryParseExpression(exe.reader, out var expression, out exe.error))
                            exe.error = $"invalid expression for string operation '{operation_name}'";
                        else
                        {
                            exe.args.Add(op);
                            exe.args.Add(expression);
                        }
                    },
                    routine: static exe =>
                    {
                        Operations op = (Operations)exe.args[0];
                        Executor expression = (Executor)exe.args[1];

                        return Executor.EExecute(
                            modify_output: data =>
                            {
                                if (data is string str)
                                    switch (op)
                                    {
                                        case Operations.mirror:
                                            return str.Mirror();
                                    }
                                return null;
                            },
                            expression.EExecute());
                    }));
            }
        }
    }
}