using System.Collections.Generic;

namespace _BOA_
{
    partial class Harbinger
    {
        static void Init_If()
        {
            AddContract(new("if", typeof(object),
                min_args: 1,
                args: static exe =>
                {
                    if (!exe.reader.TryReadChar('('))
                        exe.error ??= "expected opening parenthesis '(' for 'if' condition";
                    else if (!exe.harbinger.TryParseExpression(exe.reader, out var cond, out exe.error))
                        exe.error ??= "expected expression for 'if' condition";
                    else if (!exe.reader.TryReadChar(')'))
                        exe.error ??= "expected closing parenthesis ')' for 'if' condition";
                    else if (!exe.harbinger.TryParseBlock(exe.reader, out var block_if, out exe.error))
                        exe.error ??= "expected block after 'if' condition";
                    else
                    {
                        exe.args.Add(cond);
                        exe.args.Add(block_if);
                        if (exe.reader.TryReadMatch(out _, true, true, "else"))
                            if (exe.harbinger.TryParseBlock(exe.reader, out var block_else, out exe.error))
                                exe.args.Add(block_else);
                    }
                },
                routine: EIf));

            static IEnumerator<Contract.Status> EIf(ContractExecutor exe)
            {
                Executor cond = (Executor)exe.args[0];
                Executor block_if = (Executor)exe.args[1];
                Executor block_else = exe.args.Count > 2 ? (Executor)exe.args[2] : null;

                bool cond_result = false;

                var routine = cond.EExecute(data => cond_result = data switch
                {
                    bool b => b,
                    int i => i > 0,
                    float f => f > 0,
                    _ => data != default,
                });

                while (routine.MoveNext())
                    yield return routine.Current;

                if (cond_result)
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