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
                        else
                        {
                            exe.args.Add(op);
                            if (exe.previous_exe == null)
                                if (!exe.harbinger.TryParseExpression(exe.reader, out var expression, out exe.error))
                                    exe.error ??= $"string operation '{operation_name}' expects an expression";
                                else
                                    exe.args.Add(expression);
                        }
                    },
                    on_pipe: static (exe, data) => OnPipeData(exe, data),
                    routine: static exe =>
                    {
                        Executor expression = (Executor)exe.args[1];
                        return Executor.EExecute(
                            modify_output: data => OnPipeData(exe, data),
                            expression.EExecute());
                    }));

                static object OnPipeData(in ContractExecutor exe, in object data)
                {
                    Operations op = (Operations)exe.args[0];
                    if (data is string str)
                        switch (op)
                        {
                            case Operations.mirror:
                                return str.Mirror();
                        }
                    exe.error = $"invalid data type '{data.GetType()}' for string operation '{op}'";
                    return data;
                }
            }
        }
    }
}