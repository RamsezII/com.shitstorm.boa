using _UTIL_;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace _BOA_
{
    partial class CmdExternal
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Terminal()
        {
            Harbinger.AddContract(new("open_terminal", null,
                opts: static exe =>
                {
                    string[] options = new string[] { "-w", "--working-dir" };

                    if (exe.reader.TryReadString_matches_out(out _, false, exe.reader.lint_theme.flags, matches: options, stoppers: BoaReader._stoppers_options_))
                        if (!exe.harbinger.TryParseExpression(exe.reader, exe.scope, false, typeof(string), out var expr_wdir))
                            exe.reader.Stderr($"please specify workdir.");
                        else
                            exe.opts["workdir"] = expr_wdir;
                },
                routine: static executor =>
                {
                    bool wdir_b = executor.TryGetOptionValue("workdir", out ExpressionExecutor expr_wdir);
                    using var rout_wdir = expr_wdir?.EExecute();

                    return Executor.EExecute(
                        after_execution: data =>
                        {
                            string wdir = wdir_b
                                ? executor.harbinger.PathCheck((string)rout_wdir.Current.output, PathModes.ForceFull, false, false, out _, out _)
                                : executor.harbinger.workdir;

                            string terminal_name = null;

                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                terminal_name = "powershell.exe";
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                                terminal_name = "gnome-terminal";
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                terminal_name = "open -a Terminal";

                            if (terminal_name == null)
                                throw new PlatformNotSupportedException($"Unsupported OS platform.");

                            ProcessStartInfo psi = new()
                            {
                                FileName = terminal_name,
                                WorkingDirectory = wdir,
                                UseShellExecute = true,
                            };

                            Process.Start(psi);
                        },
                        modify_output: null,
                        rout_wdir
                    );
                }));
        }
    }
}