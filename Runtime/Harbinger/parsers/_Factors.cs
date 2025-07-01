using System;
using System.Linq;

namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseFactor_with_attribute(in BoaReader reader, in ScopeNode scope, out ExpressionExecutor expression)
        {
            if (!TryParseFactor(reader, scope, out expression, null, no_type_check: true))
                return false;
            return TryParseFactorAttribute(reader, scope, ref expression);
        }

        public bool TryParseFactor(in BoaReader reader, in ScopeNode scope, out ExpressionExecutor factor, in Type output_constraint, in bool no_type_check = false)
        {
            factor = null;

            if (reader.sig_error == null)
                if (reader.TryReadChar_match('('))
                {
                    reader.LintOpeningBraquet();
                    if (!TryParseExpression(reader, scope, false, null, out factor, type_check: false))
                    {
                        reader.Stderr("expected expression inside factor parenthesis.");
                        goto failure;
                    }
                    else if (!reader.TryReadChar_match(')', lint: reader.CloseBraquetLint()))
                    {
                        reader.Stderr($"expected closing parenthesis ')' after factor {factor.ToLog}.");
                        --reader.read_i;
                        goto failure;
                    }
                    else
                        goto success;
                }

            if (reader.sig_error == null)
                if (TryParseString(reader, scope, out var str))
                {
                    factor = str;
                    goto success;
                }
                else if (reader.sig_error != null)
                    goto failure;

            if (reader.sig_error == null)
                if (TryParseMethod(reader, scope, null, out var func_exe))
                {
                    factor = func_exe;
                    goto success;
                }
                else if (reader.sig_error != null)
                    goto failure;
                else if (TryParseVariable(reader, scope, out _, out var var_exe))
                {
                    factor = var_exe;
                    goto success;
                }
                else if (reader.sig_error != null)
                    goto failure;
                else if (reader.TryReadArgument(out string arg, lint: reader.lint_theme.fallback_default, as_function_argument: false, stoppers: BoaReader._stoppers_factors_))
                    switch (arg.ToLower())
                    {
                        case "true":
                            reader.LintToThisPosition(reader.lint_theme.constants, true);
                            factor = new LiteralExecutor(this, scope, true);
                            goto success;

                        case "false":
                            reader.LintToThisPosition(reader.lint_theme.constants, true);
                            factor = new LiteralExecutor(this, scope, false);
                            goto success;

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
                                goto failure;
                            }
                            reader.LintToThisPosition(reader.lint_theme.literal, true);
                            goto success;
                    }

                failure:
            return false;

        success:
            if (!no_type_check)
                if (factor == null)
                {
                    reader.Stderr($"can not check type on null factor.");
                    return false;
                }
                else if (factor is not ContractExecutor cont || !cont.contract.no_type_check)
                {
                    Type output_type = factor.OutputType();
                    if (((output_type == null) != (output_constraint == null)) || !output_type.IsOfType(output_constraint))
                    {
                        reader.Stderr($"expected '{output_constraint}', got '{output_type}' instead.");
                        factor = null;
                        goto failure;
                    }
                }

            return true;
        }

        internal bool TryParseFactorAttribute(in BoaReader reader, in ScopeNode scope, ref ExpressionExecutor expression)
        {
            if (!reader.TryReadChar_match('.', lint: reader.lint_theme.point))
                return true;
            else
            {
                Type output_type = expression.OutputType();

                if (output_type == null)
                {
                    reader.Stderr($"can not call attribute on a void method.");
                    return false;
                }

                var sub_conts = sub_contracts
                    .Where(pair => output_type.IsOfType(pair.Value.get_input_type?.Invoke() ?? pair.Value.input_type))
                    .Select(pair => pair.Key.name);

                if (!reader.TryReadString_matches_out(out string subcont_name, false, reader.lint_theme.sub_contracts, sub_conts, strict: false))
                {
                    reader.Stderr($"expression '.' on type '{output_type}' expects attribute or method.");
                    return false;
                }
                else if (!TryGetSubContract(output_type, subcont_name, out SubContract sub_contract))
                {
                    reader.Stderr($"type: '{output_type}' has no attribute named '{subcont_name}'.");
                    return false;
                }
                else
                {
                    expression = new SubContractExecutor(this, scope, expression, sub_contract, reader);
                    return true;
                }
            }
        }
    }
}