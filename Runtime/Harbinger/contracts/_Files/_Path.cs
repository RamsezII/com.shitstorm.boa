using _UTIL_;
using System;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Path()
        {
            AddContract(new("path", typeof(string),
                min_args: 1,
                opts: static exe =>
                {
                    FS_TYPES type = FS_TYPES.BOTH;
                    PathModes mode = PathModes.TryMaintain;

                    while (exe.reader.TryReadString_matches_out(
                        out string opt,
                        as_function_argument: false,
                        lint: exe.reader.lint_theme.flags,
                        stoppers: BoaReader._stoppers_options_,
                        matches: new string[] {
                            "-f", "--files",
                            "-d", "--directories",
                            "-m", "--mode",
                        }
                        )
                    )
                        switch (opt)
                        {
                            case "-f":
                            case "--files":
                                type = FS_TYPES.FILE;
                                break;

                            case "-d":
                            case "--directories":
                                type = FS_TYPES.DIRECTORY;
                                break;

                            case "-m":
                            case "--mode":
                                if (!exe.reader.TryReadString_matches_out(out string _mode, false, exe.reader.lint_theme.option_args, strict: false, matches: Enum.GetNames(typeof(PathModes))))
                                    exe.reader.Stderr($"specify path mode.");
                                else if (!Enum.TryParse(_mode, true, out mode))
                                    exe.reader.Stderr($"could not parse '{_mode} to {typeof(PathModes)}.");
                                break;
                        }

                    exe.opts.Add(nameof(FS_TYPES), type);
                    exe.opts.Add(nameof(PathModes), mode);
                },
                args: static exe =>
                {
                    FS_TYPES type = (FS_TYPES)exe.opts[nameof(FS_TYPES)];
                    if (exe.harbinger.TryParsePath(exe.reader, type, true, out string path))
                        exe.args.Add(path);
                    else
                        exe.reader.Stderr($"Expects expression of type path.");
                },
                function: static exe => exe.harbinger.PathCheck((string)exe.args[0], (PathModes)exe.opts[nameof(PathModes)], false, false, out _, out _)
                ));
        }
    }
}