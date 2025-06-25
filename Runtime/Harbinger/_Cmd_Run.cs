using _UTIL_;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Run()
        {
            AddContract(new("run",
                max_args: 10,
                args: static exe =>
                {
                    if (!exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out var expr))
                        exe.reader.Stderr($"Expected path expression.");
                    else
                    {
                        exe.args.Add(expr);
                        // parameters
                        while (exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out expr))
                            exe.args.Add(expr);
                    }
                },
                routine: ERun));

            static IEnumerator<Contract.Status> ERun(ContractExecutor executor)
            {
                IEnumerator<Contract.Status>[] routines = new IEnumerator<Contract.Status>[executor.args.Count];

                for (int arg_i = 0; arg_i < executor.args.Count; arg_i++)
                {
                    ExpressionExecutor expr = (ExpressionExecutor)executor.args[arg_i];
                    using var routine = routines[arg_i] = expr.EExecute();
                    while (routine.MoveNext())
                        yield return routine.Current;
                }

                string path = (string)routines[0].Current.output;
                string long_path = executor.harbinger.PathCheck(path, PathModes.ForceFull, false, false, out _, out _);
                string text = File.ReadAllText(long_path);

                if (!File.Exists(long_path))
                    executor.reader.Stderr($"can not find file at path: {long_path}.");
                else
                {
                    var harbinger = new Harbinger(executor.harbinger.shell, executor.harbinger, executor.harbinger.workdir, executor.harbinger.stdout);
                    var reader = new BoaReader(executor.reader.lint_theme, executor.reader.strict_syntax, text, long_path);

                    harbinger.args.Add(long_path);
                    for (int arg_i = 1; arg_i < routines.Length; ++arg_i)
                        harbinger.args.Add(routines[arg_i].Current.output);

                    ScopeNode scope = new ScopeNode(null, false);
                    if (false)
                        scope = scope.Dedoublate();

                    if (!harbinger.TryParseProgram(reader, scope, out var program))
                    {
                        reader.LocalizeError();
                        executor.harbinger.Stderr(reader.sig_long_error ?? reader.sig_error);
                    }
                    else
                    {
                        using var routine = program.EExecute();
                        while (routine.MoveNext())
                            yield return routine.Current;
                    }
                }
            }
        }
    }
}