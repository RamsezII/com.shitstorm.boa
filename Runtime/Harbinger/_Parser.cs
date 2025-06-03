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
                if (reader.TryReadArgument(out string arg))
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
                        {
                            Contractor cont = new(literal, reader, stdout);
                            cont.args.Add(true);
                            contractor = cont;
                        }
                        return true;

                    case "false":
                        {
                            Contractor cont = new(literal, reader, stdout);
                            cont.args.Add(false);
                            contractor = cont;
                        }
                        return true;

                    default:
                        break;
                }
            }

            return false;
        }
    }
}