namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseFactor(in BoaReader reader, in ScopeNode scope, out ExpressionExecutor factor)
        {
            factor = null;

            if (reader.sig_error == null)
                if (reader.TryReadChar_match('('))
                {
                    reader.LintOpeningBraquet();
                    if (!TryParseExpression(reader, scope, false, null, out factor, type_check: false))
                    {
                        reader.Stderr("expected expression inside factor parenthesis.");
                        return false;
                    }
                    else if (!reader.TryReadChar_match(')', lint: reader.CloseBraquetLint()))
                    {
                        reader.Stderr($"expected closing parenthesis ')' after factor {factor.ToLog}.");
                        --reader.read_i;
                        return false;
                    }
                    else
                        return true;
                }

            if (reader.sig_error == null)
                if (TryParseString(reader, scope, out var str))
                {
                    factor = str;
                    return true;
                }
                else if (reader.sig_error != null)
                    return false;

            if (reader.sig_error == null)
                if (TryParseMethod(reader, scope, null, out var func_exe))
                {
                    factor = func_exe;
                    return true;
                }
                else if (reader.sig_error != null)
                    return false;
                else if (TryParseVariable(reader, scope, out _, out var var_exe))
                {
                    factor = var_exe;
                    return true;
                }
                else if (reader.sig_error != null)
                    return false;
                else if (reader.TryReadArgument(out string arg, lint: reader.lint_theme.fallback_default, as_function_argument: false, stoppers: BoaReader._stoppers_factors_))
                    switch (arg.ToLower())
                    {
                        case "true":
                            reader.LintToThisPosition(reader.lint_theme.constants, true);
                            factor = new LiteralExecutor(this, scope, true);
                            return true;

                        case "false":
                            reader.LintToThisPosition(reader.lint_theme.constants, true);
                            factor = new LiteralExecutor(this, scope, false);
                            return true;

                        default:
                            if (arg[^1] == 'f' && Util.TryParseFloat(arg[..^1], out float _float))
                                factor = new LiteralExecutor(this, scope, _float);
                            else if (int.TryParse(arg, out int _int))
                                factor = new LiteralExecutor(this, scope, _int);
                            else if (Util.TryParseFloat(arg, out _float))
                                factor = new LiteralExecutor(this, scope, _float);
                            else
                            {
                                reader.Stderr($"unrecognized literal : '{arg}'.");
                                return false;
                            }
                            reader.LintToThisPosition(reader.lint_theme.literal, true);
                            return true;
                    }

            return false;
        }
    }
}