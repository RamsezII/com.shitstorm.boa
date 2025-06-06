using System.Collections.Generic;

namespace _BOA_
{
    partial class Harbinger
    {
        static void Init_For()
        {
            AddContract(new("for",
                args: static cont =>
                {
                    if (cont.reader.TryReadChar('('))
                        if (ParseStatement(cont.reader, null, out Contractor instr_1, out cont.error, null))
                            if (cont.reader.TryReadChar(';'))
                                if (ParseStatement(cont.reader, null, out Contractor instr_2, out cont.error, typeof(bool)))
                                    if (cont.reader.TryReadChar(';'))
                                        if (ParseStatement(cont.reader, null, out Contractor instr_3, out cont.error, null))
                                            if (cont.reader.TryReadChar(')'))
                                                if (ParseBodyOrInstruction(cont.reader, null, out AbstractContractor body, out cont.error))
                                                {
                                                    cont.args.Add(instr_1);
                                                    cont.args.Add(instr_2);
                                                    cont.args.Add(instr_3);
                                                    cont.args.Add(body);
                                                }
                },
                routine: routine_For));

            static IEnumerator<Contract.Status> routine_For(Contractor cont)
            {
                Contractor
                    instr_1 = (Contractor)cont.args[0],
                    instr_2 = (Contractor)cont.args[1],
                    instr_3 = (Contractor)cont.args[2];

                AbstractContractor
                    body = (AbstractContractor)cont.args[3];

                IEnumerator<Contract.Status> routine = instr_1.EExecute();
                while (routine.MoveNext())
                    yield return routine.Current;

                while (true)
                {
                    routine = instr_2.EExecute();
                    while (routine.MoveNext())
                        yield return routine.Current;

                    bool cond = instr_2.result.ToBool();
                    if (!cond)
                        break;

                    if (cond)
                    {
                        routine = body.EExecute();
                        while (routine.MoveNext())
                            yield return routine.Current;
                    }
                }
            }
        }
    }
}