using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_If()
        {
            AddContract(new("if",
                min_args: 1,
                no_semicolon_required: true,
                no_parenthesis: true,
                args: static exe =>
                {
                    if (!exe.reader.TryReadChar_match('('))
                        exe.error ??= "expected opening parenthesis '(' for 'if' condition";
                    else
                    {
                        exe.reader.LintOpeningBraquet();
                        var sub_scope = new ScopeNode(exe.scope);

                        if (!exe.harbinger.TryParseExpression(exe.reader, sub_scope, false, out var cond))
                            exe.error ??= "expected expression for 'if' condition";
                        else if (!exe.reader.TryReadChar_match(')', lint: exe.reader.CloseBraquetLint()))
                            exe.error ??= "expected closing parenthesis ')' for 'if' condition";
                        else if (!exe.harbinger.TryParseBlock(exe.reader, sub_scope, out var block_if))
                            exe.error ??= "expected block after 'if' condition";
                        else
                        {
                            exe.args.Add(cond);
                            exe.args.Add(block_if);

                            if (exe.reader.TryReadString_match("else", as_function_argument: false, lint: exe.reader.lint_theme.keywords))
                                if (exe.harbinger.TryParseBlock(exe.reader, sub_scope, out var block_else))
                                    exe.args.Add(block_else);
                        }
                    }
                },
                routine: EIf));

            static IEnumerator<Contract.Status> EIf(ContractExecutor exe)
            {
                Executor cond = (Executor)exe.args[0];
                Executor block_if = (Executor)exe.args[1];
                Executor block_else = exe.args.Count > 2 ? (Executor)exe.args[2] : null;

                var routine = cond.EExecute();
                while (routine.MoveNext())
                    yield return routine.Current;

                if (routine.Current.output.ToBool())
                {
                    routine = block_if.EExecute();
                    while (routine.MoveNext())
                        yield return routine.Current;
                }
                else if (block_else != null)
                {
                    routine = block_else.EExecute();
                    while (routine.MoveNext())
                        yield return routine.Current;
                }
            }
        }
    }
}