using System;

namespace _BOA_
{
    partial class Harbinger
    {
        static string TryParseInstructions(in BoaReader reader, in Action<object> stdout, out MegaContractor stack)
        {
            stack = new();
            string error;
            while (ParseInstruction(reader, stdout, out AbstractContractor contractor, out error))
                stack.stack.Add(contractor);
            return error;
        }

        static bool ParseInstruction(in BoaReader reader, in Action<object> stdout, out AbstractContractor contractor, out string error)
        {
            contractor = null;
            error = null;

            while (reader.TryPeek(out char c) && c switch
            {
                ' ' or ';' or '\n' or '\r' or '\t' => true,
                _ => false,
            })
                ++reader.read_i;

            if (reader.HasNext())
            {
                char c = reader.Peek();
                switch (c)
                {
                    case '{':
                        ++reader.read_i;
                        if (reader.HasNext())
                        {
                            MegaContractor body = new();

                            while (ParseInstruction(reader, stdout, out AbstractContractor subcontractor, out error))
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
                        break;

                    default:
                        {
                            if (reader.TryReadArgument(out string arg))
                                switch (arg.ToLower())
                                {
                                    case "if":
                                        if (ParseGetter(reader, stdout, out var cond_contractor, out error))
                                        {
                                            contractor = cond_contractor;
                                            return true;
                                        }
                                        contractor = null;
                                        return false;

                                    case "var":
                                        {
                                            if (reader.TryReadArgument(out string varname))
                                                if (reader.TryReadArgument(out string varval))
                                                    global_values[varname] = varval;
                                        }
                                        break;

                                    default:
                                        {
                                            if (global_values.TryGetValue(arg, out _))
                                            {
                                                if (reader.HasNext())
                                                    if (reader.TryReadChar('='))
                                                        if (reader.TryReadArgument(out string varval))
                                                        {
                                                            global_values[arg] = varval;
                                                            return true;
                                                        }
                                                return false;
                                            }

                                            if (global_contracts.TryGetValue(arg, out var contract))
                                            {
                                                contractor = new Contractor(contract, reader, stdout);
                                                return true;
                                            }
                                        }
                                        break;
                                }
                        }
                        break;
                }
            }

            return false;
        }

        static bool ParseGetter(BoaReader reader, Action<object> stdout, out Contractor contractor, out string error)
        {
            error = null;

            if (reader.HasNext())
            {
                Contractor _cont = null;
                string _error = null;

                if (reader.TryReadBetween('(', ')', true, () =>
                {
                    if (reader.TryReadArgument(out string arg))
                        if (global_contracts.TryGetValue(arg, out Contract contract))
                            if (contract.type == typeof(bool))
                                _cont = new Contractor(contract, reader, stdout);
                            else
                                _error = $"'if' statement expects boolean contract (while '{contract.name}' is of type {contract.type.GetType()})";
                        else
                            _error = $"no {nameof(contract)} named '{arg}'";
                    else
                        _error = $"could not parse condition";
                }))
                {
                    contractor = _cont;
                    return true;
                }

                error = _error;
            }

            contractor = null;
            return false;
        }
    }
}