namespace _BOA_
{
    partial class Harbinger
    {
        /*

            Instruction
            │
            ├── Assignation (ex: x = ...)
            │     └── Expression
            │           └── ...
            │
            └── Expression
                └── Or
                    └── And
                        └── Comparison
                            └── Addition (addition, subtraction)
                                └── Term (multiplication, division, modulo)
                                    └── Facteur
                                        ├── Littéral (nombre)
                                        ├── Variable
                                        ├── Parenthèse
                                        └── Appel de fonction

        */

        //----------------------------------------------------------------------------------------------------------

        public Executor ParseProgram(in BoaReader reader, out string error)
        {
            BlockExecutor program = new(this);

            while (TryParseBlock(reader, out Executor block, out error))
                program.stack.Add(block);

            return program;
        }

        internal bool TryParseBlock(in BoaReader reader, out Executor executor, out string error)
        {
            executor = null;
            error = null;

            if (reader.HasNext())
                if (reader.TryReadChar('{'))
                {
                    if (reader.HasNext())
                    {
                        BlockExecutor block = new(this);

                        while (TryParseBlock(reader, out Executor exe, out error))
                            block.stack.Add(exe);

                        if (error != null)
                        {
                            executor = null;
                            return false;
                        }

                        executor = block;

                        if (reader.TryReadChar('}'))
                            return true;
                        else
                            error = $"did not find closing bracket '}}'";
                    }
                }
                else if (TryParseInstruction(reader, out var instruction, out error))
                {
                    executor = instruction;
                    return true;
                }

            return false;
        }

        internal bool TryParseInstruction(in BoaReader reader, out Executor instruction, out string error)
        {
            instruction = null;
            error = null;

            if (reader.HasNext())
                if (reader.TryReadChar(';'))
                {
                    instruction = new ContractExecutor(this, null, reader);
                    return true;
                }
                else if (reader.TryReadChar('#'))
                {
                    reader.SkipUntil('\n');
                    instruction = new ContractExecutor(this, null, reader);
                    return true;
                }
                else if (TryParseExpression(reader, out var expr, out error))
                {
                    instruction = expr;
                    return true;
                }

            return false;
        }

        internal bool TryParseExpression(BoaReader reader, out ContractExecutor expression, out string error)
        {
            if (TryParseAssignation(reader, out expression, out error))
                return true;
            else if (TryParseOr(reader, out expression, out error))
                return true;
            return false;
        }

        internal bool TryParseAssignation(in BoaReader reader, out ContractExecutor assignation, out string error)
        {
            assignation = null;
            error = null;
            int read_i = reader.read_i;

            if (reader.TryReadArgument(out string varname))
                if (global_variables.TryGetValue(varname, out var variable))
                    if (reader.TryReadMatch(out string op_name, true, true, "=", "+=", "-=", "*=", "/="))
                    {
                        OperatorsM code = op_name switch
                        {
                            "=" => OperatorsM.eq,
                            "+=" => OperatorsM.add,
                            "-=" => OperatorsM.sub,
                            "*=" => OperatorsM.mul,
                            "/=" => OperatorsM.div,
                            _ => OperatorsM.unknown,
                        };

                        code |= OperatorsM.assign;

                        if (TryParseExpression(reader, out var expr, out error))
                        {
                            assignation = new ContractExecutor(this, cmd_assign_, reader, parse_arguments: false);
                            assignation.args.Add(code);
                            assignation.args.Add(variable);
                            assignation.args.Add(expr);
                            return true;
                        }
                        else
                        {
                            error ??= $"expected expression after '{op_name}' operator";
                            return false;
                        }
                    }

            reader.read_i = read_i;
            return false;
        }

        internal bool TryParseOr(in BoaReader reader, out ContractExecutor expression, out string error)
        {
            expression = null;
            if (TryParseAnd(reader, out var or1, out error))
            {
                if (reader.TryReadMatch(out string op_name, true, true, "or", "||"))
                {
                    if (TryParseAnd(reader, out var or2, out error))
                    {
                        expression = new ContractExecutor(this, cmd_math_, reader, parse_arguments: false);
                        expression.args.Add(OperatorsM.or);
                        expression.args.Add(or1);
                        expression.args.Add(or2);
                        return true;
                    }
                    else
                    {
                        error ??= $"expected right operand for '{op_name}' operator";
                        return false;
                    }
                }
                else
                {
                    expression = or1;
                    return true;
                }
            }
            return false;
        }

        internal bool TryParseAnd(in BoaReader reader, out ContractExecutor expression, out string error)
        {
            expression = null;
            if (TryParseComparison(reader, out var and1, out error))
                if (reader.TryReadMatch(out string op_name, true, true, "and", "&&"))
                {
                    if (TryParseComparison(reader, out var and2, out error))
                    {
                        expression = new ContractExecutor(this, cmd_math_, reader, parse_arguments: false);
                        expression.args.Add(OperatorsM.and);
                        expression.args.Add(and1);
                        expression.args.Add(and2);
                        return true;
                    }
                    else
                    {
                        error ??= $"expected expression after '{op_name}' operator";
                        return false;
                    }
                }
                else
                {
                    expression = and1;
                    return true;
                }
            return false;
        }

        internal bool TryParseComparison(in BoaReader reader, out ContractExecutor expression, out string error)
        {
            expression = null;
            if (TryParseAddSub(reader, out var addsub1, out error))
            {
                if (reader.TryReadChar(out char op_char, "<>=", ignore_case: true, skip_empties: true))
                {
                    OperatorsM code = op_char switch
                    {
                        '<' => OperatorsM.lt,
                        '>' => OperatorsM.gt,
                        '=' => OperatorsM.eq,
                        _ => 0,
                    };

                    if (reader.TryReadChar('=') && code != 0)
                        code |= OperatorsM.eq;

                    if (TryParseAddSub(reader, out var addsub2, out error))
                    {
                        expression = new ContractExecutor(this, cmd_math_, reader, parse_arguments: false);
                        expression.args.Add(code);
                        expression.args.Add(addsub1);
                        expression.args.Add(addsub2);
                        return true;
                    }
                    else
                    {
                        error ??= $"expected comparison after '{op_char}' operator";
                        return false;
                    }
                }
                else
                {
                    expression = addsub1;
                    return true;
                }
            }
            return false;
        }

        internal bool TryParseAddSub(in BoaReader reader, out ContractExecutor expression, out string error)
        {
            expression = null;
            if (TryParseTerm(reader, out var term1, out error))
            {
                if (reader.TryReadChar(out char op_char, "+-", ignore_case: true, skip_empties: true))
                {
                    OperatorsM code = op_char switch
                    {
                        '+' => OperatorsM.add,
                        '-' => OperatorsM.sub,
                        _ => 0,
                    };

                    if (TryParseTerm(reader, out var term2, out error))
                    {
                        expression = new ContractExecutor(this, cmd_math_, reader, parse_arguments: false);
                        expression.args.Add(code);
                        expression.args.Add(term1);
                        expression.args.Add(term2);
                        return true;
                    }
                    else
                    {
                        error ??= $"expected term after '{op_char}' operator";
                        return false;
                    }
                }
                else
                {
                    expression = term1;
                    return true;
                }
            }
            return false;
        }

        internal bool TryParseTerm(in BoaReader reader, out ContractExecutor term, out string error)
        {
            term = null;

            if (!TryParseFactor(reader, out var factor1, out error))
                return false;

            if (reader.TryReadChar(out char op_char, "*/%", ignore_case: true, skip_empties: true))
            {
                OperatorsM code = op_char switch
                {
                    '*' => OperatorsM.mul,
                    '/' => OperatorsM.div,
                    '%' => OperatorsM.mod,
                    _ => 0,
                };

                if (TryParseFactor(reader, out var factor2, out error))
                {
                    term = new ContractExecutor(this, cmd_math_, reader, parse_arguments: false);
                    term.args.Add(code);
                    term.args.Add(factor1);
                    term.args.Add(factor2);
                    return true;
                }
                else
                {
                    error ??= $"expected term after '{op_char}' operator";
                    return false;
                }
            }
            else
            {
                term = factor1;
                return true;
            }
        }

        internal bool TryParseFactor(in BoaReader reader, out ContractExecutor factor, out string error)
        {
            error = null;
            factor = null;

            if (reader.TryReadChar('('))
                if (TryParseExpression(reader, out factor, out error))
                    if (reader.TryReadChar(')'))
                        return true;
                    else
                    {
                        error ??= "expected expression inside parentheses";
                        return false;
                    }
                else
                {
                    error ??= $"expected closing parenthesis ')'";
                    return false;
                }

            if (reader.TryReadArgument(out string arg))
                if (global_contracts.TryGetValue(arg, out var contract))
                {
                    factor = new ContractExecutor(this, contract, reader);
                    if (factor.error != null)
                    {
                        error = factor.error;
                        return false;
                    }
                    return true;
                }
                else if (global_variables.TryGetValue(arg, out var variable))
                {
                    factor = new(this, cmd_variable, reader, parse_arguments: false);
                    factor.args.Add(variable);
                    return true;
                }
                else
                {
                    factor = new(this, cmd_literal, reader, parse_arguments: false);
                    string lower = arg.ToLower();
                    switch (lower)
                    {
                        case "true":
                            factor.args.Add(true);
                            return true;

                        case "false":
                            factor.args.Add(false);
                            return true;

                        default:
                            if (int.TryParse(arg, out int _int))
                                factor.args.Add(_int);
                            else if (Util.TryParseFloat(arg, out float _float))
                                factor.args.Add(_float);
                            else
                                factor.args.Add(arg);
                            return true;
                    }
                }

            return false;
        }
    }
}