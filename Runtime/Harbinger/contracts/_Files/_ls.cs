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
                    string[] flags = new string[] { "-f", "--files", "-d", "--directories", "-p", "--pattern", "-wd", "--working-dir", };
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

                            case "-p":
                            case "--pattern":
                                if (exe.harbinger.TryParseFactor(exe.reader, exe.scope, out var exe_pattern, type_check: true, output_constraint: typeof(string)))
                                    exe.opts["pattern"] = exe_pattern;
                                else
                                    exe.reader.Stderr($"please specify a pattern.");
                                break;

                            case "-wd":
                            case "--working-dir":
                                if (exe.harbinger.TryParseFactor(exe.reader, exe.scope, out var exe_wdir, type_check: true, output_constraint: typeof(string)))
                                    exe.opts["path"] = exe_wdir;
                                else
                                    exe.reader.Stderr($"expected path expression.");
                                break;
                        }
                },
                routine: static exe =>
                {
                    using var rout_patt = exe.TryGetOptionValue("pattern", out ExpressionExecutor expr_patt) ? expr_patt.EExecute() : null;

                    using var rout_wdir = exe.TryGetOptionValue("path", out ExpressionExecutor expr_wd) ? expr_wd.EExecute() : null;

                    return Executor.EExecute(
                        after_execution: null,
                        modify_output: data =>
                        {
                            string wdir = exe.harbinger.workdir;
                            if (rout_wdir != null)
                            {
                                wdir = (string)rout_wdir.Current.output;
                                wdir = exe.harbinger.PathCheck(wdir, PathModes.ForceFull, false, false, out _, out _);
                            }

                            string patt = "*";
                            if (rout_patt != null)
                                patt = (string)rout_patt.Current.output;

                            FS_TYPES type = exe.opts.TryGetValue(nameof(FS_TYPES), out var _type) ? (FS_TYPES)_type : FS_TYPES.BOTH;

                            var fsis = type switch
                            {
                                FS_TYPES.FILE => Directory.EnumerateFiles(wdir, patt),
                                FS_TYPES.DIRECTORY => Directory.EnumerateDirectories(wdir, patt),
                                _ => Directory.EnumerateFileSystemEntries(wdir, patt),
                            };

                            string join = fsis.Select(x => exe.harbinger.PathCheck(x, PathModes.ForceFull, false, false, out _, out _)).Join("\n");

                            if (string.IsNullOrWhiteSpace(join))
                                join = string.Empty;

                            return join;
                        },
                        rout_patt,
                        rout_wdir);
                }));
        }
    }
}