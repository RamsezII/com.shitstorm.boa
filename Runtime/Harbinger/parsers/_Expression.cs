using System;
using System.Linq;

namespace _BOA_
{
    partial class Harbinger
    {
        public bool TryParseExpression(in BoaReader reader, in ScopeNode scope, in bool read_as_argument, in Type output_constraint, out ExpressionExecutor expression, in bool type_check = true)
        {
            if (TryParseAssignation(reader, scope, out expression) || reader.sig_error == null && TryParseOr(reader, scope, out expression))
            {
                if (read_as_argument && !reader.TryReadChar_match(',', lint: reader.lint_theme.argument_coma) && !reader.TryPeekChar_match(')', out _))
                    if (reader.strict_syntax)
                    {
                        reader.Stderr($"expected ',' or ')' after expression.");
                        if (expression is ContractExecutor cont)
                            reader.sig_error += $" ('{cont.contract.name}')";
                        else
                            reader.sig_error += $" ('{expression.GetType()}')";
                        return false;
                    }

                if (reader.TryReadChar_match('?', lint: reader.lint_theme.operators))
                {
                    var cond = expression;
                    Type out_type = cond.OutputType();

                    if (!out_type.IsSubclassOf(typeof(bool)))
                    {
                        reader.Stderr($"expected bool expression.");
                        return false;
                    }

                    if (!TryParseExpression(reader, scope, false, null, out var _if, type_check: false))
                        reader.Stderr($"expected expression after ternary operator '?'.");
                    else if (!reader.TryReadChar_match(':', lint: reader.lint_theme.operators))
                        reader.Stderr($"expected ternary operator delimiter ':'.");
                    else if (!TryParseExpression(reader, scope, false, null, out var _else, type_check: false))
                        reader.Stderr($"expected second expression after ternary operator ':'.");
                    else
                    {
                        expression = new TernaryOpExecutor(this, scope, cond, _if, _else);
                        if (reader.sig_error != null)
                            return false;
                    }
                }

                if (!TryPipe(reader, scope, ref expression))
                {
                    reader.Stderr($"could not parse pipe statement.");
                    return false;
                }

                Type output_type = expression.OutputType();
                if (type_check)
                    if (((output_type == null) != (output_constraint == null)) || !output_type.IsOfType(output_constraint))
                    {
                        reader.Stderr($"expected expression of type '{output_constraint}', got '{output_type}' instead.");
                        expression = null;
                        return false;
                    }

                if (expression != null)
                    if (reader.TryReadChar_match('.', lint: reader.lint_theme.point))
                    {
                        if (output_type == null)
                        {
                            reader.Stderr($"void expressions have no methods.");
                            return false;
                        }

                        var sub_conts = sub_contracts.Where(pair => output_type.IsOfType(pair.Value.get_input_type?.Invoke() ?? pair.Value.input_type)).Select(pair => pair.Key.name);

                        if (!reader.TryReadString_matches_out(out string subcont_name, false, reader.lint_theme.sub_contracts, sub_conts, strict: false))
                        {
                            reader.Stderr($"type '{output_type}' has no attribute '{subcont_name}'.");
                            return false;
                        }
                        else if (!TryGetSubContract(output_type, subcont_name, out SubContract sub_contract))
                        {
                            reader.Stderr($"type: '{output_type}' has no methods '{subcont_name}'.");
                            return false;
                        }
                        else
                        {
                            expression = new SubContractExecutor(this, scope, expression, sub_contract, reader);
                            return true;
                        }
                    }

                return true;
            }
            return false;
        }

        bool TryPipe(in BoaReader reader, in ScopeNode scope, ref ExpressionExecutor current_expression)
        {
            if (!reader.TryReadChar_match('|', lint: reader.lint_theme.operators))
                return true;
            else if (current_expression.OutputType() == null)
            {
                reader.Stderr($"can not pipe void expression.");
                return false;
            }
            if (!TryParseMethod(reader, scope, current_expression, out var cont_pipe))
                reader.Stderr($"expected method after pipe symbol '|'.");
            else
            {
                current_expression = new PipeExecutor(this, scope, current_expression);

                if (!TryPipe(reader, scope, ref current_expression) || reader.sig_error != null)
                {
                    reader.Stderr($"could not parse pipe statement.");
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}