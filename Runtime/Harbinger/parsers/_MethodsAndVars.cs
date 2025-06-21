using System.Linq;

namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseMethod(in BoaReader reader, in ScopeNode scope, out ContractExecutor method_exe)
        {
            if (reader.TryReadString_matches_out(out string cont_name, as_function_argument: false, lint: reader.lint_theme.contracts, matches: global_contracts.Keys.ToArray()))
                if (!global_contracts.TryGetValue(cont_name, out var contract))
                    reader.sig_error ??= $"no contract named '{cont_name}'";
                else
                {
                    method_exe = new ContractExecutor(this, scope, contract, reader);
                    return reader.sig_error == null;
                }
            else if (reader.TryReadString_matches_out(out string func_name, as_function_argument: false, lint: reader.lint_theme.functions, matches: scope.EFuncNames().ToArray()))
                if (!scope.TryGetFunction(func_name, out var func_cont))
                    reader.sig_error ??= $"no function named '{func_name}'";
                else
                {
                    method_exe = new ContractExecutor(this, scope, func_cont, reader);
                    return reader.sig_error == null;
                }

            method_exe = null;
            return false;
        }

        internal bool TryParseVariable(in BoaReader reader, in ScopeNode scope, out VariableExecutor var_exe)
        {
            if (reader.TryReadString_matches_out(out string var_name, as_function_argument: false, lint: reader.lint_theme.variables, matches: scope.EVarNames().ToArray()))
                if (!scope.TryGetVariable(var_name, out var variable))
                    reader.sig_error ??= $"no variable named '{var_name}'";
                else
                {
                    reader.LintToThisPosition(reader.lint_theme.variables);
                    var_exe = new VariableExecutor(this, scope, variable);
                    return reader.sig_error == null;
                }
            var_exe = null;
            return false;
        }
    }
}