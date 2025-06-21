using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Unix_grep()
        {
            AddContract(new("grep",
                min_args: 1,
                args: static exe =>
                {
                    if (exe.pipe_next == null)
                        if (!exe.harbinger.TryParseString(exe.reader, exe.scope, out var expr))
                            exe.harbinger.Stderr($"expected string expression");
                        else
                            exe.arg_0 = expr;

                    if (exe.reader.TryReadArgument(out string arg, true, lint: exe.reader.lint_theme.argument))
                        exe.args.Add(arg);
                },
                routine: static exe =>
                {
                    return Executor.EExecute(
                        after_execution: null,
                        modify_output: data =>
                        {
                            string pattern = (string)exe.args[0];
                            Regex regex = new Regex(pattern);
                            return data.IterateThroughData_str().Where(x => regex.IsMatch(x)).Join("\n");
                        },
                        exe.arg_0.EExecute());
                }));
        }
    }
}