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

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_RunScript()
        {
            const string
                flag_strict = "--strict-syntax";

            Command.static_domain.AddRoutine(
                "run-script",
                min_args: 1,
                max_args: 10,
                opts: static exe =>
                {
                    if (exe.line.TryRead_one_flag(exe, flag_strict))
                        exe.opts.Add(flag_strict, null);
                },
                args: exe =>
                {
                    if (exe.line.TryReadArgument(out string script_path, out _, strict: true, path_mode: FS_TYPES.FILE))
                    {
                        exe.args.Add(script_path);
                        while (exe.line.TryReadArgument(out string arg, out _, lint: false))
                        {
                            exe.line.LintToThisPosition(Color.white);
                            exe.args.Add(arg);
                        }
                    }
                },
                routine: ERunScript);

            static IEnumerator<CMD_STATUS> ERunScript(Command.Executor exe)
            {
                bool strict_syntax = exe.opts.ContainsKey(flag_strict);

                while (!exe.line.flags.HasFlag(SIG_FLAGS.TICK))
                    yield return default;

                string script_path = (string)exe.args[0];
                script_path = exe.shell.PathCheck(script_path, _COBRA_.PathModes.ForceFull);

                if (!File.Exists(script_path))
                {
                    exe.error = $"file '{script_path}' does not exist";
                    yield break;
                }

                Harbinger harbinger = new(null, null, exe.shell.working_dir, data => exe.Stdout(data));
                BoaReader reader = BoaReader.ReadScript(LintTheme.theme_dark, strict_syntax, script_path);

                if (!harbinger.TryParseProgram(reader, null, out Executor program) || reader.sig_error != null)
                {
                    exe.error = reader.sig_long_error ?? reader.sig_error;
                    yield break;
                }

                try
                {
                    var routine = program.EExecute();

                    CMD_STATUS last_status = default;
                    while (true)
                    {
                        harbinger.shell_sig_mask = exe.line.flags;
                        if (exe.line.flags.HasFlag(SIG_FLAGS.TICK) || last_status.state == CMD_STATES.WAIT_FOR_STDIN && exe.line.flags.HasFlag(SIG_FLAGS.SUBMIT))
                        {
                        before_movenext:
                            if (routine.MoveNext())
                                switch (routine.Current.state)
                                {
                                    case Contract.Status.States.WAIT_FOR_STDIN:
                                        yield return last_status = new CMD_STATUS(CMD_STATES.WAIT_FOR_STDIN, prefixe: routine.Current.prefixe_lint);
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
                finally
                {
                    program.Dispose();
                }
            }
        }
    }
}