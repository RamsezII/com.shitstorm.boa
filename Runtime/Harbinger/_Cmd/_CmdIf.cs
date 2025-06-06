using System.Collections.Generic;

namespace _BOA_
{
    partial class Harbinger
    {
        static void Init_If()
        {
            AddContract(new("if",
                args: static cont =>
                {
                    if (cont.reader.TryReadChar('('))
                    {
                        AbstractContractor body_if = null, body_else = null;

                        void OnCond(object data)
                        {
                            if (data is bool b)
                                if (b)
                                    body_else?.Dispose();
                                else
                                    body_if.Dispose();
                        }

                        if (ParseStatement(cont.reader, OnCond, out Contractor condition, out cont.error, typeof(object)))
                            if (cont.reader.TryReadChar(')'))
                                if (ParseBodyOrInstruction(cont.reader, cont.stdout, out body_if, out cont.error))
                                {
                                    cont.args.Add(condition);
                                    cont.args.Add(body_if);

                                    if (cont.reader.HasNext())
                                        if (cont.reader.TryReadArgument("else", true))
                                            if (ParseBodyOrInstruction(cont.reader, cont.stdout, out body_else, out cont.error))
                                                cont.args.Add(body_else);
                                }
                    }
                },
                routine: routine_If));

            static IEnumerator<Contract.Status> routine_If(Contractor cont)
            {
                BodyContractor body = new();

                body.stack.Add((Contractor)cont.args[0]);
                body.stack.Add((AbstractContractor)cont.args[1]);

                if (cont.args.Count >= 3)
                    body.stack.Add((AbstractContractor)cont.args[2]);

                return body.EExecute();
            }


            AddContract(new("for",
                args: static cont =>
                {
                    if (cont.reader.TryReadChar('('))
                    {
                        Contractor
                            instr_1 = null, instr_2 = null, instr_3 = null;

                        AbstractContractor
                            body = null;

                        void OnCond(object data)
                        {
                        }

                        if (ParseStatement(cont.reader, null, out instr_1, out cont.error, null))
                            if (cont.reader.TryReadChar(';'))
                                if (ParseStatement(cont.reader, OnCond, out instr_2, out cont.error, typeof(bool)))
                                    if (cont.reader.TryReadChar(';'))
                                        if (ParseStatement(cont.reader, null, out instr_3, out cont.error, null))
                                            if (cont.reader.TryReadChar(')'))
                                                if (ParseBodyOrInstruction(cont.reader, null, out body, out cont.error))
                                                {
                                                    cont.args.Add(instr_1);
                                                    cont.args.Add(instr_2);
                                                    cont.args.Add(instr_3);
                                                    cont.args.Add(body);
                                                }
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

                var routine = instr_1.EExecute();
                while (routine.MoveNext())
                    yield return routine.Current;
            }
        }
    }
}