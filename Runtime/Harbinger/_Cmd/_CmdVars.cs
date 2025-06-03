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
                                if (!ParseStatement(cont.reader, data => global_values[varname] = data, out Contractor getter, out cont.error, null))
                                    cont.args.Add(varname);
                },
                action: static cont =>
                {

                }));
        }
    }
}