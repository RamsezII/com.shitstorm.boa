using System.Collections.Generic;

namespace _BOA_
{
    partial class Harbinger
    {
        static void Init_Vars()
        {
            AddContract(new("var",
                args: static cont =>
                {
                    if (cont.reader.TryReadArgument(out string varname))
                        if (cont.reader.HasNext())
                            if (cont.reader.TryReadChar('='))
                                if (ParseStatement(cont.reader, null, out var statement, out cont.error, null))
                                {
                                    global_values[varname] = new Variable<object>(varname, null);
                                    cont.args.Add(varname);
                                    cont.args.Add(statement);
                                }
                },
                routine: routine_EVar));

            static IEnumerator<Contract.Status> routine_EVar(Contractor cont)
            {
                string varname = (string)cont.args[0];
                AbstractContractor statement = (AbstractContractor)cont.args[1];

                var routine = statement.EExecute();
                while (routine.MoveNext())
                    yield return routine.Current;

                global_values[varname].value = statement.result;
            }
        }
    }
}