using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_Foreach()
        {
            AddContract(new("foreach",
                min_args: 2,
                function_style_arguments: false,
                no_parenthesis: true,
                no_semicolon_required: true,
                args: static exe =>
                {
                    //bool got_parenthesis = exe.reader.TryReadChar_match('(');

                    if (!exe.reader.TryReadArgument(out string var_name, false))
                        exe.error ??= "specify an item name";
                    if (!exe.reader.TryReadString_match("in"))
                        exe.error ??= $"expected 'in' keyword";
                    else if (!exe.harbinger.TryParseExpression(exe.reader, exe, false, out var expr_list))
                        exe.error ??= "expected expression to iterate through";
                    else
                    {
                        exe._variables.Add(var_name, new BoaVariable(null));
                        if (!exe.harbinger.TryParseBlock(exe.reader, exe, out var block))
                            exe.error ??= "expected instruction (or block of instructions) after ')' in 'for' instruction";
                        else
                        {
                            exe.args.Add(expr_list);
                            exe.args.Add(var_name);
                            exe.args.Add(block);
                        }
                    }
                },
                routine: EFor));

            static IEnumerator<Contract.Status> EFor(ContractExecutor exe)
            {
                Executor expr_list = (Executor)exe.args[0];
                string var_name = (string)exe.args[1];
                Executor block = (Executor)exe.args[2];

                var routine_list = expr_list.EExecute();
                while (routine_list.MoveNext())
                    yield return routine_list.Current;

                BoaVariable variable = exe._variables.Get(var_name);
                List<object> list = (List<object>)routine_list.Current.output;

                for (int i = 0; i < list.Count; i++)
                {
                    variable.value = list[i];

                    var routine_block = block.EExecute();
                    while (routine_block.MoveNext())
                        yield return routine_block.Current;
                }
            }
        }
    }
}