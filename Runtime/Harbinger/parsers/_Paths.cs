using _UTIL_;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        public bool TryParsePath(in BoaReader reader, in FS_TYPES type, in bool read_as_argument, out string path)
        {
            if (reader.TryParseString(out path, read_as_argument))
            {
                reader.LintToThisPosition(reader.lint_theme.paths, true);
                goto success;
            }
            else if (reader.TryReadArgument(out path, false, reader.lint_theme.paths, stoppers: BoaReader._stoppers_paths))
                goto success;

            failure:
            path = null;
            reader.Stderr($"could not parse path '{path}'.");
            return false;

        success:
            if (reader.sig_error != null)
                goto failure;

            if (reader.IsOnCursor())
            {
                reader.completion_l = reader.completion_r = null;

                try
                {
                    string long_path = shell.PathCheck(path, PathModes.ForceFull, out bool is_rooted, out bool is_local_to_shell);
                    DirectoryInfo parent = Directory.GetParent(long_path);

                    if (parent != null)
                    {
                        reader.completion_l = shell.PathCheck(parent.FullName, is_rooted ? PathModes.ForceFull : PathModes.TryLocal);

                        if (parent.Exists)
                        {
                            string path_r;

                            if (type == FS_TYPES.DIRECTORY)
                                path_r = parent.EnumerateDirectories().FirstOrDefault().FullName;
                            else
                                path_r = parent.EnumerateFileSystemInfos().FirstOrDefault().FullName;

                            if (path_r != null)
                                reader.completion_r = shell.PathCheck(path_r, is_rooted ? PathModes.ForceFull : PathModes.TryLocal);

                            if (signal.flags.HasFlag(SIG_FLAGS_new.CHANGE))
                            {
                                var paths = type switch
                                {
                                    FS_TYPES.DIRECTORY => parent.EnumerateDirectories(),
                                    _ => parent.EnumerateFileSystemInfos(),
                                };

                                foreach (var fsi in paths)
                                    reader.completions_v.Add(shell.PathCheck(fsi.FullName, is_rooted ? PathModes.ForceFull : PathModes.TryLocal));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    //Debug.LogException(e);
                }
            }

            return true;
        }
    }
}