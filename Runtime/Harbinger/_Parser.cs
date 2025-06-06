using System;

namespace _BOA_
{
    partial class Harbinger
    {
        public static string TryParseInstructions(in BoaReader reader, in Action<object> stdout, out BodyContractor stack)
        {
            stack = new();
            string error;
            while (ParseBodyOrInstruction(reader, stdout, out AbstractContractor contractor, out error))
                stack.stack.Add(contractor);
            return error;
        }

        public static bool ParseBodyOrInstruction(in BoaReader reader, in Action<object> stdout, out AbstractContractor contractor, out string error)
        {
            contractor = null;
            error = null;

            if (reader.HasNext())
                if (reader.TryReadChar('{'))
                {
                    if (reader.HasNext())
                    {
                        BodyContractor body = new();

                        while (ParseBodyOrInstruction(reader, stdout, out AbstractContractor subcontractor, out error))
                            body.stack.Add(subcontractor);

                        if (error != null)
                        {
                            contractor = null;
                            return false;
                        }

                        contractor = body;

                        if (reader.TryReadChar('}'))
                            return true;
                        else
                            error = $"did not find closing bracket '}}'";
                    }
                }
                else if (ParseInstruction(reader, stdout, out contractor, out error))
                    return true;

            return false;
        }

        public static bool ParseInstruction(in BoaReader reader, in Action<object> stdout, out AbstractContractor contractor, out string error)
        {
            contractor = null;
            error = null;

            if (reader.TryReadArgument(out string arg))
            {
                if (global_values.ContainsKey(arg))
                {
                    if (reader.TryPeek(out char c))
                        switch (c)
                        {
                            case '=':
                                return ParseStatement(reader, data => global_values[arg] = new Variable<object>(arg, data), out contractor, out error, typeof(object));
                        }
                    return false;
                }

                reader.read_i = reader.start_i;

                if (ParseStatement(reader, stdout, out contractor, out error, null))
                    return true;
            }

            return false;
        }

        public static bool ParseStatement(BoaReader reader, Action<object> stdout, out AbstractContractor contractor, out string error, in Type force_type)
        {
            error = null;

            if (reader.HasNext())
                if (reader.TryReadChar(';'))
                {
                    contractor = null;
                    return force_type == null;
                }
                else if (reader.TryReadArgument(out string arg))
                {
                    if (global_contracts.TryGetValue(arg, out Contract contract) && (force_type == null || force_type.IsAssignableFrom(contract.type)))
                    {
                        contractor = new Contractor(contract, reader, stdout);
                        return true;
                    }

                    reader.read_i = reader.start_i;

                    if (ParseData(reader, stdout, out Literal<object> literal, out error, force_type))
                    {
                        contractor = new Contractor_value<object>(literal);
                        return true;
                    }
                }

            contractor = null;
            return false;
        }

        public static bool ParseData(BoaReader reader, Action<object> stdout, out Literal<object> literal, out string error, in Type force_type)
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