using _COBRA_;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Shell()
        {
            Command.static_domain.AddRoutine("harbinger", routine: ERoutine);

            static IEnumerator<CMD_STATUS> ERoutine(Command.Executor cobra_exe)
            {
                bool debug = true;
                string prefixe = ">";

                var scope = new ScopeNode(null);

                CMD_STATUS shell_status = new(CMD_STATES.WAIT_FOR_STDIN, prefixe: prefixe);

                while (true)
                {
                    int read_old = cobra_exe.line.read_i;
                    if (cobra_exe.line.TryReadAll(out string input_line, lint: false))
                    {
                        var harbinger = new Harbinger(null, null, data => cobra_exe.Stdout(data));
                        var reader = BoaReader.ReadLines(LintTheme.theme_dark, false, cursor_i: cobra_exe.line.cursor_i, lines: input_line);

                        ScopeNode scope1 = scope;
                        if (!cobra_exe.line.flags.HasFlag(SIG_FLAGS.SUBMIT))
                            scope1 = scope.Dedoublate();

                        bool success = harbinger.TryParseProgram(reader, scope1, out var program);

                        if (debug)
                            if (cobra_exe.line.HasFlags_any(SIG_FLAGS.TAB | SIG_FLAGS.ALT))
                            {
                                char[] chars = new char[1 + reader.text.Length];
                                for (int i = 0; i < chars.Length; i++)
                                    if (i == reader.cpl_end)
                                        chars[i] = '²';
                                    else if (i == reader.cpl_start)
                                        chars[i] = '°';
                                    else
                                        chars[i] = ' ';
                                string str = new(chars);

                                Debug.Log($"{reader.text}\n{str}");
                                Debug.Log($"{reader.completions.Count} completions ({cobra_exe.line.flags}) -> {reader.completions.Join(" ")}");
                            }

                        if (!success)
                        {
                            cobra_exe.line.LintToThisPosition(cobra_exe.line.read_i - read_old, reader.GetLintResult(LintTheme.lint_default));

                            string error = reader.long_error ?? reader.error ?? $"could not parse command {{ {input_line} }}";
                            if (cobra_exe.line.flags.HasFlag(SIG_FLAGS.SUBMIT))
                                cobra_exe.Stdout(error, error.SetColor(Color.orange));
                        }
                        else
                        {
                            cobra_exe.line.LintToThisPosition(cobra_exe.line.read_i - read_old, reader.GetLintResult(LintTheme.lint_default));
                            if (cobra_exe.line.flags.HasFlag(SIG_FLAGS.SUBMIT))
                            {
                                var routine = program.EExecute();
                                CMD_STATUS last_status = default;

                                while (true)
                                {
                                    harbinger.shell_sig_mask = cobra_exe.line.flags;

                                    if (last_status.state == CMD_STATES.WAIT_FOR_STDIN)
                                        if (!cobra_exe.line.TryReadAll(out harbinger.shell_stdin))
                                            harbinger.shell_stdin = null;

                                    if (cobra_exe.line.flags.HasFlag(SIG_FLAGS.TICK) || last_status.state == CMD_STATES.WAIT_FOR_STDIN && cobra_exe.line.flags.HasFlag(SIG_FLAGS.SUBMIT))
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
                                            break;
                                    }
                                    else
                                        yield return last_status;
                                }
                            }
                        }
                    }
                    yield return shell_status;
                }
            }
        }
    }
}