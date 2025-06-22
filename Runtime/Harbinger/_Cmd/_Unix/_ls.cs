using _UTIL_;
using System.IO;
using System.Linq;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Unix_ls()
        {
            AddContract(new("ls",
                outputs_if_end_of_instruction: true,
                opts: static exe =>
                {
                    if (exe.reader.TryReadString_matches_out(out string flag, false, lint: exe.reader.lint_theme.flags, matches: new string[] { "f", "d", }))
                    {
                        FS_TYPES type = flag switch
                        {
                            "f" => FS_TYPES.FILE,
                            "d" => FS_TYPES.DIRECTORY,
                            _ => FS_TYPES.BOTH,
                        };
                        exe.opts.Add(nameof(FS_TYPES), type);
                    }

                    if (exe.reader.TryReadString_match("pattern", false, lint: exe.reader.lint_theme.options))
                        if (!exe.reader.TryParseString(out string arg, false))
                            exe.reader.Stderr($"please specify a pattern (flag '--pattern'");
                        else
                            exe.opts.Add("pattern", arg);
                },
                function: static exe =>
                {
                    FS_TYPES type = exe.opts.TryGetValue(nameof(FS_TYPES), out var _type) ? (FS_TYPES)_type : FS_TYPES.BOTH;
                    string pattern = exe.opts.TryGetValue("pattern", out var _pattern) ? (string)_pattern : "*";

                    var fsis = type switch
                    {
                        FS_TYPES.FILE => Directory.EnumerateFiles(exe.harbinger.shell.working_dir, pattern),
                        FS_TYPES.DIRECTORY => Directory.EnumerateDirectories(exe.harbinger.shell.working_dir, pattern),
                        _ => Directory.EnumerateFileSystemEntries(exe.harbinger.shell.working_dir, pattern),
                    };

                    string join = fsis.Select(x => exe.harbinger.shell.PathCheck(x, PathModes.TryLocal)).Join("\n");

                    if (string.IsNullOrWhiteSpace(join))
                        join = string.Empty;

                    return join;
                }));
        }
    }
}