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
                else if (ParseInstruction(reader, stdout, out Contractor cont, out error))
                {
                    contractor = cont;
                    return true;
                }

            return false;
        }

        public static bool ParseInstruction(in BoaReader reader, in Action<object> stdout, out Contractor contractor, out string error)
        {
            contractor = null;
            error = null;

            if (reader.TryReadArgument(out string arg))
            {
                if (global_values.ContainsKey(arg))
                {
                    if (reader.HasNext())
                        if (reader.TryReadChar('='))
                            if (ParseStatement(reader, data => global_values[arg] = data, out Contractor cont, out error, typeof(object)))
                            {
                                contractor = cont;
                                return true;
                            }
                    return false;
                }

                reader.read_i = reader.start_i;

                if (ParseStatement(reader, stdout, out contractor, out error, null))
                    return true;
            }

            return false;
        }

        public static bool ParseStatement(BoaReader reader, Action<object> stdout, out Contractor contractor, out string error, in Type force_type)
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

                    if (ParseLiteral(reader, stdout, out contractor, out error, force_type))
                        return true;
                }

            contractor = null;
            return false;
        }

        public static bool ParseLiteral(BoaReader reader, Action<object> stdout, out Contractor contractor, out string error, in Type force_type)
        {
            contractor = null;
            error = null;

            if (reader.TryReadArgument(out string arg))
            {
                string lower = arg.ToLower();
                switch (lower)
                {
                    case "true":
                        contractor = new Contractor(null, reader, stdout)
                        {
                            result = true,
                        };
                        return true;

                    case "false":
                        contractor = new Contractor(null, reader, stdout)
                        {
                            result = false,
                        };
                        return true;

                    default:
                        if (int.TryParse(arg, out int _int))
                            contractor = new Contractor(null, reader, stdout)
                            {
                                result = _int,
                            };
                        else if (Util.TryParseFloat(arg, out float _float))
                            contractor = new Contractor(null, reader, stdout)
                            {
                                result = _float,
                            };
                        else
                            contractor = new Contractor(null, reader, stdout)
                            {
                                result = arg,
                            };
                        return true;
                }
            }

            return false;
        }
    }
}