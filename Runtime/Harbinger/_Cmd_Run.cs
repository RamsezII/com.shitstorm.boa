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
                    if (exe.reader.TryReadArgument(out string path, true, lint: exe.reader.lint_theme.paths))
                    {
                        string long_path = Path.Combine(Directory.GetParent(exe.reader.script_path).FullName, path);
                        if (!File.Exists(long_path))
                            exe.reader.sig_error ??= $"can not find file at path: {long_path}";
                        else
                        {
                            List<Executor> args_exprs = new();
                            var harbinger = new Harbinger(exe.harbinger.shell, exe.harbinger, exe.harbinger.stdout);
                            var reader = BoaReader.ReadScript(exe.reader.lint_theme, exe.reader.strict_syntax, long_path);

                            while (exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out var expr))
                            {
                                harbinger.args.Add(null);
                                args_exprs.Add(expr);
                            }

                            if (!harbinger.TryParseProgram(reader, exe.scope, out var program))
                                exe.reader.sig_error = exe.reader.sig_long_error;

                            exe.args.Add(program);
                            exe.args.Add(args_exprs);
                        }
                    }
                },
                routine: ERoutine));

            static IEnumerator<Contract.Status> ERoutine(ContractExecutor exe)
            {
                Executor program = (Executor)exe.args[0];
                List<Executor> args_exprs = (List<Executor>)exe.args[1];

                for (int i = 0; i < args_exprs.Count; i++)
                {
                    var arg_routine = args_exprs[i].EExecute();
                    while (arg_routine.MoveNext())
                        yield return arg_routine.Current;
                    program.harbinger.args[i] = arg_routine.Current.output;
                }

                var program_routine = program.EExecute();
                while (program_routine.MoveNext())
                    yield return program_routine.Current;
            }
        }
    }
}