using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_For()
        {
            AddContract(new("for",
                min_args: 1,
                function_style_arguments: false,
                no_parenthesis: true,
                no_semicolon_required: true,
                args: static exe =>
                {
                    if (!exe.reader.TryReadChar_match('('))
                        exe.error ??= "expected '(' at the beginning of 'for' instruction";
                    else if (!exe.harbinger.TryParseInstruction(exe.reader, exe, true, out var instr_init))
                        exe.error ??= "expected instruction after '(' in 'for' instruction";
                    else
                    {
                        if (!exe.harbinger.TryParseExpression(exe.reader, exe, false, out var cond))
                            exe.error ??= "expected expression after first instruction in 'for' instruction";
                        else
                        {
                            exe.reader.TryReadChar_match(';');
                            if (!exe.harbinger.TryParseInstruction(exe.reader, exe, false, out var instr_loop))
                                exe.error ??= "expected instruction after second expression in 'for' instruction";
                            else if (!exe.reader.TryReadChar_match(')'))
                                exe.error ??= "expected ')' at the end of 'for' instruction";
                            else if (!exe.harbinger.TryParseBlock(exe.reader, exe, out var block))
                                exe.error ??= "expected instruction (or block of instructions) after ')' in 'for' instruction";
                            else
                            {
                                exe.args.Add(instr_init);
                                exe.args.Add(cond);
                                exe.args.Add(instr_loop);
                                exe.args.Add(block);
                            }
                        }
                    }
                },
                routine: EFor));

            static IEnumerator<Contract.Status> EFor(ContractExecutor exe)
            {
                Executor instr_init = (Executor)exe.args[0];
                Executor cond = (Executor)exe.args[1];
                Executor instr_loop = (Executor)exe.args[2];
                Executor block = (Executor)exe.args[3];

                var routine = instr_init.EExecute();
                while (routine.MoveNext())
                    yield return routine.Current;

                while (true)
                {
                    routine = cond.EExecute();
                    while (routine.MoveNext())
                        yield return routine.Current;

                    if (!routine.Current.output.ToBool())
                        break;

                    routine = block.EExecute();
                    while (routine.MoveNext())
                        yield return routine.Current;

                    routine = instr_loop.EExecute();
                    while (routine.MoveNext())
                        yield return routine.Current;
                }
            }
        }
    }
}