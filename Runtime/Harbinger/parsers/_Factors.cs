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
                    if (!TryParseExpression(reader, scope, false, out factor))
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
                if (TryParseMethod(reader, scope, out var func_exe))
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
                else if (reader.TryReadArgument(out string arg, lint: LintTheme.lint_default, as_function_argument: false))
                    switch (arg.ToLower())
                    {
                        case "true":
                            reader.LintToThisPosition(reader.lint_theme.constants);
                            factor = new LiteralExecutor(this, scope, literal: true);
                            return true;

                        case "false":
                            reader.LintToThisPosition(reader.lint_theme.constants);
                            factor = new LiteralExecutor(this, scope, literal: false);
                            return true;

                        default:
                            if (int.TryParse(arg, out int _int))
                                factor = new LiteralExecutor(this, scope, literal: _int);
                            else if (Util.TryParseFloat(arg, out float _float))
                                factor = new LiteralExecutor(this, scope, literal: _float);
                            else
                            {
                                reader.Stderr($"unrecognized literal : '{arg}'.");
                                return false;
                            }
                            reader.LintToThisPosition(reader.lint_theme.literal);
                            return true;
                    }

            return false;
        }
    }
}