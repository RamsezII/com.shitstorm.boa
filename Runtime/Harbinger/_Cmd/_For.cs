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
                        exe.reader.sig_error ??= "expected '(' at the beginning of 'for' instruction";
                    else
                    {
                        exe.reader.LintOpeningBraquet();
                        ScopeNode sub_scope = new ScopeNode(exe.scope);
                        if (!exe.harbinger.TryParseInstruction(exe.reader, sub_scope, true, out var instr_init))
                            exe.reader.sig_error ??= "expected instruction after '(' in 'for' instruction";
                        else
                        {
                            if (!exe.harbinger.TryParseExpression(exe.reader, sub_scope, false, out var cond))
                                exe.reader.sig_error ??= "expected expression after first instruction in 'for' instruction";
                            else
                            {
                                exe.reader.TryReadChar_match(';', lint: exe.reader.lint_theme.command_separators);
                                if (!exe.harbinger.TryParseInstruction(exe.reader, sub_scope, false, out var instr_loop))
                                    exe.reader.sig_error ??= "expected instruction after second expression in 'for' instruction";
                                else if (!exe.reader.TryReadChar_match(')', lint: exe.reader.CloseBraquetLint()))
                                    exe.reader.sig_error ??= "expected ')' at the end of 'for' instruction";
                                else if (!exe.harbinger.TryParseBlock(exe.reader, sub_scope, out var block))
                                    exe.reader.sig_error ??= "expected instruction (or block of instructions) after ')' in 'for' instruction";
                                else
                                {
                                    exe.args.Add(instr_init);
                                    exe.args.Add(cond);
                                    exe.args.Add(instr_loop);
                                    exe.args.Add(block);
                                }
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