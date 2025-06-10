using System.Collections.Generic;

namespace _BOA_
{
    partial class Harbinger
    {
        static void Init_Cmd()
        {
            AddContract(new("for",
                min_args: 1,
                args: static exe =>
                {
                    if (!exe.reader.TryReadChar('('))
                        exe.error = "expected '(' at the beginning of 'for' instruction";
                    else if (!exe.harbinger.TryParseInstruction(exe.reader, out var instr_init, out exe.error))
                        exe.error = "expected instruction after '(' in 'for' instruction";
                    else
                    {
                        exe.reader.TryReadChar(';');
                        if (!exe.harbinger.TryParseExpression(exe.reader, out var cond, out exe.error))
                            exe.error = "expected expression after first instruction in 'for' instruction";
                        else
                        {
                            exe.reader.TryReadChar(';');
                            if (!exe.harbinger.TryParseInstruction(exe.reader, out var instr_loop, out exe.error))
                                exe.error = "expected instruction after second expression in 'for' instruction";
                            else if (!exe.reader.TryReadChar(')'))
                                exe.error = "expected ')' at the end of 'for' instruction";
                            else if (!exe.harbinger.TryParseBlock(exe.reader, out var block, out exe.error))
                                exe.error = "expected block after ')' in 'for' instruction";
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

                bool loop = true;
                while (loop)
                {
                    routine = cond.EExecute(data => loop = data.ToBool());
                    while (routine.MoveNext())
                        yield return routine.Current;

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