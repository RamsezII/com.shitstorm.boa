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
                    if (exe.pipe_previous == null)
                        if (!exe.harbinger.TryParseString(exe.reader, exe.scope, out var expr))
                            exe.harbinger.Stderr($"expected string expression");
                        else
                            exe.arg_0 = expr;

                    if (exe.reader.TryParseString(out string pattern, true))
                        exe.args.Add(pattern);
                    else
                        exe.reader.Stderr($"please specify a grep pattern");
                },
                routine: static exe =>
                {
                    return Executor.EExecute(
                        after_execution: null,
                        modify_output: data =>
                        {
                            string pattern = (string)exe.args[0];
                            Regex regex = new Regex(pattern.GlobToRegex());
                            string join = data.IterateThroughData_str().Where(x => regex.IsMatch(x)).Join("\n");

                            if (string.IsNullOrWhiteSpace(join))
                                return null;
                            else
                                return join;
                        },
                        exe.arg_0.EExecute());
                }));

            AddContract(new("regex",
                min_args: 1,
                args: static exe =>
                {
                    if (exe.pipe_previous == null)
                        if (!exe.harbinger.TryParseString(exe.reader, exe.scope, out var expr))
                            exe.harbinger.Stderr($"expected string expression");
                        else
                            exe.arg_0 = expr;

                    if (exe.reader.TryParseString(out string pattern, true))
                        exe.args.Add(pattern);
                    else
                        exe.reader.Stderr($"please specify a regex pattern");
                },
                routine: static exe =>
                {
                    return Executor.EExecute(
                        after_execution: null,
                        modify_output: data =>
                        {
                            string pattern = (string)exe.args[0];
                            Regex regex = new Regex(pattern);
                            string join = data.IterateThroughData_str().Where(x => regex.IsMatch(x)).Join("\n");

                            if (string.IsNullOrWhiteSpace(join))
                                return null;
                            else
                                return join;
                        },
                        exe.arg_0.EExecute());
                }));
        }
    }
}