namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseFactor(in BoaReader reader, in ScopeNode scope, out ExpressionExecutor factor)
        {
            factor = null;

            if (reader.error == null)
                if (reader.TryReadChar_match('('))
                {
                    reader.LintOpeningBraquet();
                    if (!TryParseExpression(reader, scope, false, out factor))
                    {
                        reader.error ??= "expected expression inside factor parenthesis";
                        return false;
                    }
                    else if (!reader.TryReadChar_match(')', lint: reader.CloseBraquetLint()))
                    {
                        reader.error ??= $"expected closing parenthesis ')' after factor {factor.ToLog}";
                        --reader.read_i;
                        return false;
                    }
                    else
                        return true;
                }

            if (reader.error == null)
                if (TryParseString(reader, out string str))
                {
                    factor = new LiteralExecutor(this, scope, literal: str);
                    return true;
                }

            if (reader.error == null)
                if (TryParseMethod(reader, scope, out var func_exe))
                {
                    factor = func_exe;
                    return true;
                }
                else if (reader.error != null)
                    return false;
                else if (TryParseVariable(reader, scope, out var var_exe))
                {
                    factor = var_exe;
                    return true;
                }
                else if (reader.error != null)
                    return false;
                else if (reader.TryReadArgument(out string arg, lint: LintTheme.lint_default, as_function_argument: false))
                    if (scope.TryGetVariable(arg, out var variable))
                    {
                        reader.LintToThisPosition(reader.lint_theme.variables);
                        factor = new VariableExecutor(this, scope, variable);
                        return true;
                    }
                    else
                        switch (arg.ToLower())
                        {
                            case "true":
                                factor = new LiteralExecutor(this, scope, literal: true);
                                return true;

                            case "false":
                                factor = new LiteralExecutor(this, scope, literal: false);
                                return true;

                            default:
                                if (int.TryParse(arg, out int _int))
                                    factor = new LiteralExecutor(this, scope, literal: _int);
                                else if (Util.TryParseFloat(arg, out float _float))
                                    factor = new LiteralExecutor(this, scope, literal: _float);
                                else
                                {
                                    reader.error ??= $"unrecognized literal : '{arg}'";
                                    return false;
                                }
                                return true;
                        }

            return false;
        }
    }
}