using System.Collections;
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
            AddContract(new("list", typeof(List<object>),
                max_args: 100,
                args: static exe =>
                {
                    while (exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(object), out var expr))
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

            AddSubContract(new("add", typeof(List<object>), typeof(object),
                min_args: 1,
                args: static exe =>
                {
                    if (exe.pipe_previous == null)
                        if (exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(object), out var expr_item))
                            exe.arg_0 = expr_item;
                        else
                            exe.reader.Stderr("expeted item to add to the list.");
                },
                routine: static exe =>
                {
                    ExpressionExecutor expr_list = ((SubContractExecutor)exe).output_exe;

                    using var rout_list = expr_list.EExecute();
                    using var rout_item = exe.arg_0.EExecute();

                    return Executor.EExecute(
                        after_execution: null,
                        modify_output: data =>
                        {
                            List<object> list = (List<object>)rout_list.Current.output;
                            object item = rout_item.Current.output;
                            list.Add(item);
                            return list;
                        },
                        rout_list,
                        rout_item);
                }));
        }
    }
}