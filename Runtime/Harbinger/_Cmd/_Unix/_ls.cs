using _UTIL_;
using System.IO;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Unix_ls()
        {
            AddContract(new("ls",
                opts: static exe =>
                {
                    FS_TYPES type = FS_TYPES.BOTH;
                    if (exe.reader.TryReadString_matches_out(out string flag, false, lint: exe.reader.lint_theme.flags, matches: new string[] { "f", "d", }))
                        switch (flag)
                        {
                            case "f":
                                type = FS_TYPES.FILE;
                                break;

                            case "d":
                                type = FS_TYPES.DIRECTORY;
                                break;
                        }
                    exe.args.Add(type);
                },
                function: static exe =>
                {
                    FS_TYPES type = (FS_TYPES)exe.args[0];
                    var fsis = type switch
                    {
                        FS_TYPES.FILE => Directory.EnumerateFiles(exe.harbinger.shell.working_dir),
                        FS_TYPES.DIRECTORY => Directory.EnumerateDirectories(exe.harbinger.shell.working_dir),
                        _ => Directory.EnumerateFileSystemEntries(exe.harbinger.shell.working_dir),
                    };
                    string join = fsis.Join("\n");
                    if (exe.pipe_next == null)
                        exe.harbinger.shell.AddLine(join);
                    return join;
                }));
        }
    }
}