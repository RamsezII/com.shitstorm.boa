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
                            var harbinger = new Harbinger(exe.harbinger.stdout, long_path, exe.harbinger.strict_syntax);
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
        }
    }
}