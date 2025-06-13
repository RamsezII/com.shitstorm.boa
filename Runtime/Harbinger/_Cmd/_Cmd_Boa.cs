using _COBRA_;
using _UTIL_;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        public SIG_FLAGS shell_sig_mask;
        public string shell_stdin;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Run()
        {
            const string
                flag_strict = "--strict-syntax";

            Command.static_domain.AddRoutine(
                "run-boa-script",
                min_args: 1,
                opts: static exe =>
                {
                    if (exe.line.TryRead_one_flag(exe, flag_strict))
                        exe.opts.Add(flag_strict, null);
                },
                args: exe =>
                {
                    if (exe.line.TryReadArgument(out string script_path, out _, strict: true, path_mode: FS_TYPES.FILE))
                        exe.args.Add(script_path);
                },
                routine: ERunScript);

            static IEnumerator<CMD_STATUS> ERunScript(Command.Executor exe)
            {
                bool strict_syntax = exe.opts.ContainsKey(flag_strict);

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
                BoaReader reader = new(strict_syntax, script_text);
                using Executor program = harbinger.ParseProgram(reader, out string error);

                if (error != null)
                {
                    exe.error = reader.LocalizeError(error, File.ReadAllLines(script_path));
                    yield break;
                }

                var routine = program.EExecute();

                CMD_STATUS last_status = default;
                while (true)
                {
                    harbinger.shell_sig_mask = exe.line.flags;

                    if (last_status.state == CMD_STATES.WAIT_FOR_STDIN)
                        if (!exe.line.TryReadAll(out harbinger.shell_stdin))
                            harbinger.shell_stdin = null;

                    if (exe.line.flags.HasFlag(SIG_FLAGS.TICK) || last_status.state == CMD_STATES.WAIT_FOR_STDIN && exe.line.flags.HasFlag(SIG_FLAGS.SUBMIT))
                    {
                    before_movenext:
                        if (routine.MoveNext())
                            switch (routine.Current.state)
                            {
                                case Contract.Status.States.WAIT_FOR_STDIN:
                                    yield return last_status = new CMD_STATUS(CMD_STATES.WAIT_FOR_STDIN, prefixe: routine.Current.prefixe);
                                    break;

                                case Contract.Status.States.ACTION_skip:
                                    goto before_movenext;

                                default:
                                    yield return last_status = new CMD_STATUS(progress: routine.Current.progress);
                                    break;
                            }
                        else
                            yield break;
                    }
                    else
                        yield return last_status;
                }
            }
        }
    }
}