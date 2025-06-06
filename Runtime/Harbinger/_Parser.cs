using System;

namespace _BOA_
{
    partial class Harbinger
    {
        public static string TryParseProgram(in BoaReader reader, in Action<object> stdout, out BodyContractor body)
        {
            body = new();
            string error;
            while (TryParseBlock(reader, stdout, out AbstractContractor contractor, out error))
                body.stack.Add(contractor);
            return error;
        }

        public static bool TryParseBlock(in BoaReader reader, in Action<object> stdout, out AbstractContractor block, out string error)
        {
            block = null;
            error = null;

            if (reader.HasNext())
                if (reader.TryReadChar('{'))
                {
                    if (reader.HasNext())
                    {
                        BodyContractor body = new();

                        while (TryParseBlock(reader, stdout, out AbstractContractor sub_block, out error))
                            body.stack.Add(sub_block);

                        if (error != null)
                        {
                            block = null;
                            return false;
                        }

                        block = body;

                        if (reader.TryReadChar('}'))
                            return true;
                        else
                            error = $"did not find closing bracket '}}'";
                    }
                }
                else if (TryParseExpression(reader, stdout, out block, out error))
                    return true;

            return false;
        }

        public static bool TryParseExpression(BoaReader reader, Action<object> stdout, out AbstractContractor expression, out string error, in Type force_type = null)
        {
            error = null;

            if (reader.HasNext())
                if (reader.TryReadChar(';'))
                {
                    expression = null;
                    return true;
                }
                else if (reader.TryReadArgument(out string arg0))
                {
                    if (global_contracts.TryGetValue(arg0, out Contract contract) && (force_type == null || force_type.IsAssignableFrom(contract.type)))
                    {
                        expression = new Contractor(contract, reader, stdout);
                        return true;
                    }

                    reader.read_i = reader.start_i;

                    if (ParseValue(reader, stdout, out Literal<object> literal, out error, force_type))
                        if (reader.TryReadArgument(out string arg1))
                        {
                            Contract cmd = arg1 switch
                            {
                                "=" => cmd_assign,
                                "+=" => cmd_increment,
                                "==" => cmd_equals,
                                "!=" => cmd_nequals,
                                ">" => cmd_greater,
                                "<" => cmd_lesser,
                                ">=" => cmd_egreater,
                                "<=" => cmd_elesser,
                                _ => null,
                            };

                            if (cmd == cmd_increment)
                            {
                                if (global_values.TryGetValue(arg0, out var variable))
                                {
                                    var contractor = new Contractor(cmd, reader, stdout, parse_arguments: false);
                                    contractor.args.Add(variable);
                                    contractor.args.Add(literal);
                                    expression = contractor;
                                    return true;
                                }
                            }
                            else if (ParseValue(reader, stdout, out Literal<object> literal2, out error, force_type))
                            {
                                var contractor = new Contractor(cmd, reader, stdout, parse_arguments: false);
                                contractor.args.Add(literal);
                                contractor.args.Add(literal2);
                                expression = contractor;
                                return true;
                            }
                        }
                        else
                        {
                            expression = new Contractor_value<object>(literal);
                            return true;
                        }
                }

            expression = null;
            return false;
        }

        public static bool ParseValue(BoaReader reader, Action<object> stdout, out Literal<object> literal, out string error, in Type force_type)
        {
            error = null;

            if (reader.TryReadArgument(out string arg))
                if (global_values.TryGetValue(arg, out var variable))
                {
                    literal = new Literal<object>(variable);
                    return true;
                }
                else
                {
                    string lower = arg.ToLower();
                    switch (lower)
                    {
                        case "true":
                            literal = new(true);
                            return true;

                        case "false":
                            literal = new(false);
                            return true;

                        default:
                            if (int.TryParse(arg, out int _int))
                                literal = new(_int);
                            else if (Util.TryParseFloat(arg, out float _float))
                                literal = new(_float);
                            else
                                literal = new(arg);
                            return true;
                    }
                }

            literal = null;
            return false;
        }
    }
}