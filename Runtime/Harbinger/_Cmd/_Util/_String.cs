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
                toString,
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
                        if (exe.pipe_previous == null && !exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out expr))
                            exe.reader.Stderr($"'{exe.contract.name}' expects an expression.");
                        else
                        {
                            Operations op = 0;
                            if (exe.reader.TryReadString_matches_out(out string op_name, true, ignore_case: true, lint: exe.reader.lint_theme.operators, matches: Enum.GetNames(typeof(Operations))))
                            {
                                exe.reader.Stderr("missing string operation.");
                                if (!Enum.TryParse(op_name, true, out op))
                                    exe.reader.Stderr($"unknown string operation '{op_name}'.");
                            }
                            exe.arg_0 = expr;
                            exe.args.Add(op);
                        }
                    },
                    routine: static exe =>
                    {
                        Operations op = (Operations)exe.args[0];
                        return Executor.EExecute(
                            null, data =>
                            {
                                if (data is string str)
                                    switch (op)
                                    {
                                        case Operations.mirror:
                                            return str.Mirror();
                                        default:
                                            return str;
                                    }
                                else if (op == Operations.toString)
                                    return $"{data}";
                                exe.harbinger.Stderr($"invalid data type '{data?.GetType()}' for string operation '{op}'");
                                return data;
                            },
                            exe.arg_0.EExecute());
                    }));
            }
        }
    }
}