using _UTIL_;
using System.IO;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Unix()
        {
            AddContract(new("cd",
                min_args: 1,
                args: static exe =>
                {
                    if (exe.harbinger.TryParsePath(exe.reader, FS_TYPES.DIRECTORY, out string path))
                        exe.arg_0 = new LiteralExecutor(exe.harbinger, exe.scope, path);
                    else
                        exe.reader.Stderr($"expected path string.");
                },
                routine: static exe =>
                {
                    return Executor.EExecute(
                        after_execution: data =>
                        {
                            string path = data is string str ? str : data.ToString();
                            path = exe.harbinger.shell.PathCheck(path, PathModes.ForceFull);

                            if (Directory.Exists(path))
                                exe.harbinger.shell.ChangeWorkdir(path);
                            else
                                exe.harbinger.Stderr($"Found no directory at path '{path}'.");
                        },
                        modify_output: null,
                        exe.arg_0.EExecute()
                    );
                }));

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

            AddContract(new("cat",
                args: static exe =>
                {
                    if (exe.harbinger.TryParsePath(exe.reader, FS_TYPES.FILE, out string path))
                    {
                        path = exe.harbinger.shell.PathCheck(path, PathModes.ForceFull);
                        exe.arg_0 = new LiteralExecutor(exe.harbinger, exe.scope, path);
                    }
                },
                routine: static exe =>
                {
                    return Executor.EExecute(
                        after_execution: null,
                        modify_output: data =>
                        {
                            string path = data is string str ? str : data.ToString();
                            if (File.Exists(path))
                            {
                                string cat = File.ReadAllText(path).Trim();
                                if (exe.pipe_next == null)
                                    exe.harbinger.shell.AddLine(cat);
                                return cat;
                            }
                            exe.harbinger.Stderr($"Could not find file '{path}'.");
                            return null;
                        },
                        exe.arg_0.EExecute());
                }));
        }
    }
}