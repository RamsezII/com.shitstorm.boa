using _UTIL_;
using System.Collections.Generic;
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
            AddContract(new("ls", typeof(List<string>),
                outputs_if_end_of_instruction: true,
                opts: static exe =>
                {
                    string[] flags = new string[] { "-f", "--files", "-d", "--directories", "--pattern", "-wd", "--working-dir", };
                    while (exe.reader.TryReadString_matches_out(out string flag, false, lint: exe.reader.lint_theme.flags, stoppers: BoaReader._stoppers_options_, matches: flags))
                        switch (flag)
                        {
                            case "-f":
                            case "--files":
                            case "-d":
                            case "--directories":
                                {
                                    FS_TYPES type = flag switch
                                    {
                                        "-f" or "--files" => FS_TYPES.FILE,
                                        "-d" or "--directories" => FS_TYPES.DIRECTORY,
                                        _ => FS_TYPES.BOTH,
                                    };
                                    exe.opts[nameof(FS_TYPES)] = type;
                                }
                                break;

                            case "--pattern":
                                if (!exe.reader.TryParseString(out string arg, false))
                                    exe.reader.Stderr($"please specify a pattern.");
                                else
                                    exe.opts["pattern"] = arg;
                                break;

                            case "-wd":
                            case "--working-dir":
                                if (exe.harbinger.TryParsePath(exe.reader, FS_TYPES.DIRECTORY, false, out string path))
                                    exe.opts["path"] = exe.harbinger.PathCheck(path, PathModes.ForceFull, false, false, out _, out _);
                                else
                                    exe.reader.Stderr($"expected path expression.");
                                break;
                        }
                },
                function: static exe =>
                {
                    FS_TYPES type = exe.opts.TryGetValue(nameof(FS_TYPES), out var _type) ? (FS_TYPES)_type : FS_TYPES.BOTH;
                    string pattern = exe.opts.TryGetValue("pattern", out var _pattern) ? (string)_pattern : "*";

                    string workdir = exe.harbinger.workdir;
                    if (exe.opts.TryGetValue("path", out var path))
                        workdir = (string)path;

                    var fsis = type switch
                    {
                        FS_TYPES.FILE => Directory.EnumerateFiles(workdir, pattern),
                        FS_TYPES.DIRECTORY => Directory.EnumerateDirectories(workdir, pattern),
                        _ => Directory.EnumerateFileSystemEntries(workdir, pattern),
                    };

                    string join = fsis.Select(x => exe.harbinger.PathCheck(x, PathModes.ForceFull, false, false, out _, out _)).Join("\n");

                    if (string.IsNullOrWhiteSpace(join))
                        join = string.Empty;

                    return join;
                }));
        }
    }
}