namespace _BOA_
{
    partial class Harbinger
    {
        static Contract
            cmd_assign,
            cmd_increment;

        static Contract
            cmd_equals,
            cmd_nequals,
            cmd_greater,
            cmd_egreater,
            cmd_lesser,
            cmd_elesser;

        //----------------------------------------------------------------------------------------------------------

        static void Init_Vars()
        {
            cmd_assign = AddContract(new("var",
                args: static cont =>
                {
                    if (cont.reader.TryReadArgument(out string varname))
                        if (cont.reader.HasNext())
                            if (cont.reader.TryReadChar('='))
                                if (TryParseExpression(cont.reader, null, out var statement, out cont.error, null))
                                {
                                    global_values[varname] = new Variable<object>(varname, null);
                                    cont.args.Add(varname);
                                    cont.args.Add(statement);
                                }
                },
                routine: static cont =>
                {
                    string varname = (string)cont.args[0];
                    AbstractContractor statement = (AbstractContractor)cont.args[1];
                    return statement.ERoutinize(() => global_values[varname].value = statement.result);
                }));

            cmd_increment = AddContract(new("increment",
                args: static cont =>
                {
                    if (cont.reader.TryReadArgument(out string varname))
                        if (global_values.TryGetValue(varname, out var variable))
                            if (TryParseExpression(cont.reader, null, out var expression, out cont.error, null))
                            {
                                cont.args.Add(variable);
                                cont.args.Add(expression);
                            }
                },
                routine: static cont =>
                {
                    Variable<object> variable = (Variable<object>)cont.args[0];
                    AbstractContractor expression = (AbstractContractor)cont.args[1];
                    return expression.ERoutinize(() =>
                    {
                        switch (variable.value)
                        {
                            case int _int:
                                variable.value = _int + (int)expression.result;
                                break;
                            case float _float:
                                variable.value = _float + (float)expression.result;
                                break;
                            default:
                                cont.error = $"error increment";
                                break;
                        }
                    });
                }));

            cmd_equals = AddContract(new Contract("equals",
                min_args: 2,
                action: static cont =>
                {
                    AbstractContractor expr1 = (AbstractContractor)cont.args[0];
                    AbstractContractor expr2 = (AbstractContractor)cont.args[1];

                    var routine = expr1.EExecute();
                    routine.MoveNext();

                    routine = expr2.EExecute();
                    routine.MoveNext();

                    cont.stdout(expr1.result.Equals(expr2.result));
                }));

            cmd_lesser = AddContract(new Contract("lesser",
                min_args: 2,
                action: static cont =>
                {
                    AbstractContractor expr1 = (AbstractContractor)cont.args[0];
                    AbstractContractor expr2 = (AbstractContractor)cont.args[1];

                    var routine = expr1.EExecute();
                    routine.MoveNext();

                    routine = expr2.EExecute();
                    routine.MoveNext();

                    if (int.TryParse(expr1.result.ToString(), out int i1) && int.TryParse(expr2.result.ToString(), out int i2))
                        cont.stdout(i1 < i2);
                    else if (Util.TryParseFloat(expr1.result.ToString(), out float f1) && Util.TryParseFloat(expr2.result.ToString(), out float f2))
                        cont.stdout(f1 < f2);
                    else
                        cont.error = $"parse error";
                }));
        }
    }
}