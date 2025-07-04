﻿using System;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_Var()
        {
            AddContract(new("var", typeof(object),
                get_output_type: static exe => exe.arg_0.OutputType(),
                function_style_arguments: false,
                args: static exe =>
                {
                    if (!exe.reader.TryReadArgument(out string var_name, as_function_argument: false, lint: exe.reader.lint_theme.variables))
                        exe.reader.Stderr($"Expected variable name after 'var'.");
                    else
                    {
                        Type type = typeof(object);

                        if (exe.pipe_previous != null)
                            type = exe.pipe_previous.OutputType();
                        else if (!exe.reader.TryReadChar_match('=', lint: exe.reader.lint_theme.operators))
                            exe.reader.Stderr($"Expected '=' after variable name '{var_name}'.");
                        else if (!exe.harbinger.TryParseExpression(exe.reader, exe.scope, false, typeof(object), out var expr))
                            exe.reader.Stderr($"Failed to parse expression after '=' for variable '{var_name}'.");
                        else
                        {
                            exe.arg_0 = expr;
                            type = expr.OutputType() ?? type;
                        }

                        if (exe.reader.sig_error != null)
                            return;

                        BoaVariable variable = new(null, type);
                        if (!exe.scope.TryAddVariable(var_name, variable))
                            exe.reader.Stderr($"Could not declare variable '{var_name}'.");
                        exe.args.Add(var_name);
                    }
                },
                routine: static exe =>
                {
                    string var_name = (string)exe.args[0];
                    return Executor.EExecute(
                        after_execution: data => exe.scope.SetVariable(var_name, new(data)),
                        stack: exe.arg_0.EExecute());
                }));
        }
    }
}