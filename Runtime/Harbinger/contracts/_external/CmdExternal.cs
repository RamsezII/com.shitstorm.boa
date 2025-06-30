using _ARK_;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _BOA_
{
    static partial class CmdExternal
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Harbinger.AddContract(new("os_cmdline", typeof(object),
                min_args: 1,
                outputs_if_end_of_instruction: true,
                opts: static exe =>
                {
                    string[] options = new string[] { "-a", "--async", "-w", "--working-dir", "-n", "--no-log", };

                    exe.reader.completions_v.Clear();

                    while (exe.reader.TryReadString_matches_out(out string flag, false, exe.reader.lint_theme.flags, matches: options, stoppers: BoaReader._stoppers_options_))
                        switch (flag)
                        {
                            case "-a":
                            case "--async":
                                exe.opts["async"] = null;
                                break;

                            case "-w":
                            case "--working-dir":
                                if (!exe.harbinger.TryParseExpression(exe.reader, exe.scope, false, typeof(string), out var expr_wdir))
                                    exe.reader.Stderr($"please specify workdir.");
                                else
                                    exe.opts["workdir"] = expr_wdir;
                                break;
                        }
                },
                args: static exe =>
                {
                    if (exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(string), out var expr_cmdline))
                        exe.arg_0 = expr_cmdline;
                    else
                        exe.reader.Stderr($"expected string expression.");
                },
                routine: static exe => exe.opts.ContainsKey("async") ? ERunCmd_async(exe) : ERunCmd_sync(exe)
                ));

            static IEnumerator<Contract.Status> ERunCmd_sync(ContractExecutor executor)
            {
                bool wdir_b = executor.TryGetOptionValue("workdir", out ExpressionExecutor expr_wdir);
                using var rout_wdir = expr_wdir?.EExecute();

                return Executor.EExecute(
                    after_execution: null,
                    modify_output: data =>
                    {
                        string wdir = wdir_b
                            ? executor.harbinger.PathCheck((string)rout_wdir.Current.output, PathModes.ForceFull, false, false, out _, out _)
                            : executor.harbinger.workdir;

                        StringBuilder sb = new();

                        Util.RunExternalCommand(wdir, (string)data, on_stdout: stdout => sb.AppendLine(stdout));

                        return sb.ToString();
                    },
                    rout_wdir,
                    executor.arg_0.EExecute());
            }

            static IEnumerator<Contract.Status> ERunCmd_async(ContractExecutor executor)
            {
                bool log_b = !executor.opts.ContainsKey("no-log");
                string wdir = executor.harbinger.workdir;

                if (executor.TryGetOptionValue("workdir", out ExpressionExecutor expr_wdir))
                {
                    using var rout_wdir = expr_wdir.EExecute();
                    while (rout_wdir.MoveNext())
                        yield return rout_wdir.Current;
                    wdir = (string)rout_wdir.Current.output;
                }

                using var rout_cmd = executor.arg_0.EExecute();
                while (rout_cmd.MoveNext())
                    yield return rout_cmd.Current;
                string cmdline = (string)rout_cmd.Current.output;

                string output = string.Empty;
                object _lock = new();

                using var task = Task.Run(() => Util.RunExternalCommand_streaming(wdir, cmdline, on_stdout: stdout =>
                {
                    lock (_lock)
                        output += stdout + "\n";
                }));

                while (!task.IsCompleted)
                {
                    string temp;
                    lock (_lock)
                        temp = output;
                    yield return new Contract.Status(Contract.Status.States.BLOCKING, prefixe_text: temp);
                }

                lock (_lock)
                    yield return new Contract.Status(Contract.Status.States.ACTION_skip, output: output);
            }
        }
    }
}