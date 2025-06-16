using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_While()
        {
            AddContract(new("while",
                function_style_arguments: false,
                no_semicolon_required: true,
                no_parenthesis: true,
                args: static exe =>
                {
                    if (!exe.reader.TryReadChar_match('('))
                        exe.error = "expected opening parenthesis '(' for 'while' condition";
                    else
                    {
                        exe.reader.LintOpeningBraquet();
                        if (!exe.harbinger.TryParseExpression(exe.reader, exe, false, out var cond))
                            exe.error ??= "expected expression for 'while' condition";
                        else if (!exe.reader.TryReadChar_match(')', lint: exe.reader.CloseBraquetLint()))
                            exe.error = "expected closing parenthesis ')' for 'while' condition";
                        else if (!exe.harbinger.TryParseBlock(exe.reader, exe, out var block))
                            exe.error ??= "expected an instruction, or a block of instructions after 'while' condition";
                        else
                        {
                            exe.args.Add(cond);
                            exe.args.Add(block);
                        }
                    }
                },
                routine: EWhile));

            static IEnumerator<Contract.Status> EWhile(ContractExecutor exe)
            {
                Executor cond = (Executor)exe.args[0];
                Executor block = (Executor)exe.args[1];

                while (true)
                {
                    var routine = cond.EExecute();
                    while (routine.MoveNext())
                        yield return routine.Current;

                    if (!routine.Current.output.ToBool())
                        break;

                    routine = block.EExecute();
                    while (routine.MoveNext())
                        yield return routine.Current;
                }
            }
        }
    }
}