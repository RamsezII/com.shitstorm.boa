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
                    if (exe.reader.TryReadString_match("d", false, exe.reader.lint_theme.options, add_to_completions: false))
                        if (!exe.harbinger.TryParsePath(exe.reader, FS_TYPES.DIRECTORY, false, out string wdir))
                            exe.reader.Stderr($"please specify workdir.");
                        else
                            exe.opts.Add("d", wdir);
                },
                action: static exe =>
                {
                    if (exe.TryGetOptionValue("d", out string wdir))
                        wdir = exe.harbinger.PathCheck(wdir, PathModes.ForceFull, false, false, out _, out _);
                    else
                        wdir = exe.harbinger.workdir;

                    ProcessStartInfo psi = new()
                    {
                        FileName = GetTerminalCommand(),
                        WorkingDirectory = wdir,
                        UseShellExecute = true,
                    };
                    Process.Start(psi);

                    static string GetTerminalCommand()
                    {
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            return "powershell.exe";
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            return "gnome-terminal";
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            return "open -a Terminal";

                        throw new PlatformNotSupportedException($"Unsupported OS platform.");
                    }
                }));
        }
    }
}