namespace _BOA_
{
    partial class Harbinger
    {
        static string TryParse(in string text, out MegaContractor stack)
        {
            stack = new();
            int read_i = 0;
            string error;
            while (ParseContractor(text, ref read_i, out AbstractContractor contractor, out error))
                stack.stack.Add(contractor);
            return error;
        }

        static bool ParseContractor(in string text, ref int read_i, out AbstractContractor contractor, out string error)
        {
            error = null;

            if (Util_cobra.HasNext(text, ref read_i))
                switch (text[read_i])
                {
                    case '{':
                        ++read_i;
                        if (Util_cobra.HasNext(text, ref read_i))
                        {
                            MegaContractor body = new();

                            while (ParseContractor(text, ref read_i, out AbstractContractor subcontractor, out error))
                                body.stack.Add(subcontractor);

                            if (error != null)
                            {
                                contractor = null;
                                return false;
                            }

                            contractor = body;

                            if (text[read_i] == '}')
                            {
                                ++read_i;
                                return true;
                            }
                            else
                                error = $"did not find closing bracket '}}'";
                        }
                        break;

                    default:
                        {
                            while (text[read_i] switch
                            {
                                ' ' or ';' or '\n' or '\r' or '\t' => true,
                                _ => false,
                            })
                                ++read_i;

                            if (Util_boa.TryReadArgument(text, out int start_i, ref read_i, out string arg))
                            {
                                if (global_contracts.TryGetValue(arg, out Contract contract))
                                {
                                    contractor = new Contractor(contract, text, ref read_i);
                                    return true;
                                }
                            }
                        }
                        break;
                }

            contractor = null;
            return false;
        }
    }
}