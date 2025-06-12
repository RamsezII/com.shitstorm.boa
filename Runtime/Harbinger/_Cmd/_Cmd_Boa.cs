using _COBRA_;
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
            const string
                as_cmd_line = "--as-command-line";

            Command.static_domain.AddRoutine(
                "run-boa-script",
                min_args: 1,
                opts: static exe =>
                {
                    if (exe.line.TryRead_one_flag(exe, as_cmd_line))
                        exe.opts.Add(as_cmd_line, null);
                },
                args: exe =>
                {
                    if (exe.line.TryReadArgument(out string script_path, out _, strict: true, path_mode: FS_TYPES.FILE))
                        exe.args.Add(script_path);
                },
                routine: ERunScript);

            static IEnumerator<CMD_STATUS> ERunScript(Command.Executor exe)
            {
                bool asCmdLine_flag = exe.opts.ContainsKey(as_cmd_line);

                while (!exe.line.flags.HasFlag(SIG_FLAGS.TICK))
                    yield return default;

                string script_path = (string)exe.args[0];
                script_path = exe.shell.PathCheck(script_path, PathModes.ForceFull);

                if (!File.Exists(script_path))
                {
                    exe.error = $"file '{script_path}' does not exist";
                    yield break;
                }

                string script_text = File.ReadAllText(script_path);
                Harbinger harbinger = new(data => exe.Stdout(data));
                BoaReader reader = new(asCmdLine_flag ? BoaReader.Sources.CommandLine : BoaReader.Sources.Script, script_text);
                using Executor executor = harbinger.ParseProgram(reader, out string error);

                if (error != null)
                {
                    exe.error = reader.LocalizeError(error, File.ReadAllLines(script_path));
                    yield break;
                }
                else if (false)
                    Debug.Log(script_text);

                using var routine = executor.EExecute();
                while (true)
                    if (!exe.line.flags.HasFlag(SIG_FLAGS.TICK))
                        yield return default;
                    else
                    {
                    before_iter:
                        if (routine.MoveNext())
                            switch (routine.Current.state)
                            {
                                case Contract.Status.States.WAIT_FOR_STDIN:
                                    yield return new CMD_STATUS(CMD_STATES.WAIT_FOR_STDIN, prefixe: routine.Current.prefixe);
                                    break;
                                case Contract.Status.States.ACTION_skippable:
                                    goto before_iter;
                                default:
                                    yield return new CMD_STATUS(progress: routine.Current.progress);
                                    break;
                            }
                        else
                            yield break;
                    }
            }
        }
    }
}