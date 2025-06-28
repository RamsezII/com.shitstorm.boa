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
            Harbinger.AddContract(new("os_cmdline", null,
                min_args: 1,
                outputs_if_end_of_instruction: true,
                opts: static exe =>
                {
                    if (exe.reader.TryReadString_matches_out(out string flag, false, exe.reader.lint_theme.flags, add_to_completions: false, matches: new string[] { "a", "s", }))
                        exe.opts.Add("mode", flag);
                    else
                    {
                        exe.reader.Stderr($"expects flag 's' or 'a' (synchronous or asynchronous).");
                        return;
                    }

                    if (exe.reader.TryReadString_match("d", false, exe.reader.lint_theme.options, add_to_completions: false))
                        if (!exe.harbinger.TryParseExpression(exe.reader, exe.scope, false, typeof(string), out var expr_wdir))
                            exe.reader.Stderr($"please specify workdir.");
                        else
                            exe.opts.Add("d", expr_wdir);
                },
                args: static exe =>
                {
                    if (exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(string), out var expr_cmdline))
                        exe.arg_0 = expr_cmdline;
                    else
                        exe.reader.Stderr($"expected string expression.");
                },
                routine: static exe => exe.opts["mode"] switch
                {
                    "s" => ERunCmd_sync(exe),
                    "a" => ERunCmd_async(exe),
                    _ => null,
                }));

            static IEnumerator<Contract.Status> ERunCmd_sync(ContractExecutor executor)
            {
                bool wdir_b = executor.TryGetOptionValue("d", out ExpressionExecutor expr_wdir);
                using var rout_wdir = expr_wdir?.EExecute();

                return Executor.EExecute(
                    after_execution: null,
                    modify_output: data =>
                    {
                        string wdir = wdir_b
                            ? executor.harbinger.PathCheck((string)rout_wdir.Current.output, PathModes.ForceFull, false, false, out _, out _)
                            : executor.harbinger.workdir;
                        StringBuilder sb = new();
                        Util.RunExternalCommand(wdir, (string)data, on_stdout: stdout =>
                        {
                            sb.Append(stdout);
                        });
                        return sb.ToString();
                    },
                    rout_wdir,
                    executor.arg_0.EExecute());
            }

            static IEnumerator<Contract.Status> ERunCmd_async(ContractExecutor executor)
            {
                string wdir = executor.harbinger.workdir;

                if (executor.TryGetOptionValue("d", out ExpressionExecutor expr_wdir))
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

                StringBuilder sb = new();

                using var task = Task.Run(() => Util.RunExternalCommandBlockingStreaming(wdir, cmdline, on_stdout: stdout =>
                {
                    NUCLEOR.instance.ToMainThread(() => executor.harbinger.stdout(stdout));
                    sb.Append(stdout);
                }));

                while (!task.IsCompleted)
                    yield return new Contract.Status(Contract.Status.States.BLOCKING, "running external command...");

                yield return new Contract.Status(Contract.Status.States.ACTION_skip, output: sb.ToString());
            }
        }
    }
}