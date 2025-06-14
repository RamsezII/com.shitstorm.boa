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
                    if (exe.reader.TryReadArgument(out string path, true))
                    {
                        string long_path = Path.Combine(Directory.GetParent(exe.harbinger.script_path).FullName, path);
                        if (!File.Exists(long_path))
                            exe.error ??= $"can not find file at path: {long_path}";
                        else
                        {
                            var harbinger = new Harbinger(exe.harbinger, exe.harbinger.stdout, long_path, exe.harbinger.strict_syntax);

                            while (exe.harbinger.TryParseExpression(exe.reader, exe.caller, true, out var expr))
                                harbinger.args.Add(expr);

                            if (!harbinger.TryRunScript(out var program, out exe.reader.error, out exe.reader.long_error))
                                exe.error = exe.reader.long_error;
                            else
                                exe.args.Add(program);
                        }
                    }
                },
                routine: static exe =>
                {
                    Executor program = (Executor)exe.args[0];
                    return program.EExecute();
                }));

            AddContract(new("args",
                min_args: 1,
                args: static exe =>
                {
                    if (exe.reader.TryReadArgument(out string arg, true))
                        exe.args.Add(int.Parse(arg));
                },
                routine: static exe =>
                {
                    int arg_i = (int)exe.args[0];
                    if (arg_i >= exe.harbinger.args.Count)
                        return null;
                    ExpressionExecutor expr = (ExpressionExecutor)exe.harbinger.args[arg_i];
                    return expr.EExecute();
                }));
        }
    }
}