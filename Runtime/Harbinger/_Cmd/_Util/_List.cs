using System.Collections.Generic;
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

            AddContract(new("append",
                min_args: 1,
                args: static exe =>
                {
                    if (exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out var expr_list))
                        exe.args.Add(expr_list);
                    else
                        exe.reader.Stderr("expected expression of type List.");

                    if (exe.pipe_previous == null)
                        if (exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out var expr_item))
                            exe.arg_0 = expr_item;
                        else
                            exe.reader.Stderr("expeted item to add to the list.");
                },
                routine: static exe =>
                {
                    ExpressionExecutor expr_list = (ExpressionExecutor)exe.args[0];
                    using var expr_routine = expr_list.EExecute();
                    using var expr_item = exe.arg_0.EExecute();

                    return Executor.EExecute(
                        after_execution: null,
                        modify_output: data =>
                        {
                            List<object> list = (List<object>)expr_routine.Current.output;
                            object item = expr_item.Current.output;
                            list.Add(item);
                            return list;
                        },
                        expr_routine,
                        expr_item);
                }));
        }
    }
}