using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        static void InitContracts()
        {
            Init_Vars();
            Init_If();
            Init_For();

            AddContract(new(
                "echo",
                typeof(object),
                min_args: 1,
                args: static cont =>
                {
                    if (ParseStatement(cont.reader, null, out var statement, out cont.error, null))
                        cont.args.Add(statement);
                },
                routine: static cont =>
                {
                    AbstractContractor statement = (AbstractContractor)cont.args[0];
                    return statement.ERoutinize(() => cont.stdout(statement.result));
                }));

            AddContract(new(
                "sleep",
                min_args: 1,
                args: static cont =>
                {
                    if (cont.reader.TryReadArgument(out string arg))
                        cont.args.Add(arg);
                },
                routine: EWait)
                );

            static IEnumerator<Contract.Status> EWait(Contractor contractor)
            {
                float timer = 0;

                while (timer < 1)
                {
                    timer += Time.unscaledDeltaTime;
                    yield return new() { progress = timer, };
                }
            }
        }
    }
}