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

                        if (TryParseExpression(cont.reader, OnCond, out var condition, out cont.error, typeof(object)))
                            if (cont.reader.TryReadChar(')'))
                                if (TryParseBlock(cont.reader, cont.stdout, out body_if, out cont.error))
                                {
                                    cont.args.Add(condition);
                                    cont.args.Add(body_if);

                                    if (cont.reader.HasNext())
                                        if (cont.reader.TryReadArgument("else", true))
                                            if (TryParseBlock(cont.reader, cont.stdout, out body_else, out cont.error))
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
        }
    }
}