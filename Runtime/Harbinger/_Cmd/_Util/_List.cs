﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_List()
        {
            AddContract(new("list",
                max_args: 100,
                args: static exe =>
                {
                    while (exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out var expr))
                        exe.args.Add(expr);
                },
                routine: static exe =>
                {
                    var routines = exe.args.Select(expr => ((Executor)expr).EExecute()).ToArray();
                    return Executor.EExecute(
                        modify_output: data =>
                        {
                            List<object> list = new(routines.Length);
                            for (int i = 0; i < routines.Length; ++i)
                                list.Add(routines[i].Current.output);
                            return list;
                        },
                        stack: routines
                        );
                }));
        }
    }
}