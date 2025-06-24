using _UTIL_;
using System.IO;
using System.Linq;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        public bool TryParsePath(in BoaReader reader, in FS_TYPES type, in bool read_as_argument, out string path)
        {
            bool force_quotes = false;
            if (reader.TryParseString(out path, read_as_argument))
                force_quotes = true;
            else if (reader.sig_error == null)
                reader.TryReadArgument(out path, false, reader.lint_theme.strings, stoppers: BoaReader._stoppers_paths);

            if (reader.sig_error != null)
                goto failure;

            int read_old = reader.read_i;
            reader.HasNext();
            if (!reader.IsOnCursor())
                reader.read_i = read_old;
            else
            {
                reader.stop_completing = true;
                reader.completion_l = reader.completion_r = null;

                try
                {
                    if (string.IsNullOrWhiteSpace(path))
                        path = "./";

                    string long_path = PathCheck(path, PathModes.ForceFull, false, false, out bool is_rooted, out bool is_local_to_shell);
                    bool ends_with_bar = long_path.EndsWith('/', '\\');

                    if (ends_with_bar)
                        long_path = long_path[..^1];

                    PathModes path_mode = is_rooted ? PathModes.ForceFull : PathModes.TryLocal;
                    DirectoryInfo parent = ends_with_bar ? new(long_path) : Directory.GetParent(long_path);

                    if (parent != null)
                    {
                        reader.completion_l = PathCheck(ends_with_bar ? long_path : parent.FullName, path_mode, true, true, out _, out _);

                        if (parent.Exists)
                        {
                            if (!ends_with_bar)
                                reader.completion_r = (PathCheck(long_path, path_mode, false, false, out _, out _) + "/").QuoteStringSafely();
                            else
                            {
                                DirectoryInfo current = new(long_path);
                                if (current != null && current.Exists)
                                {
                                    string path_r;
                                    if (type == FS_TYPES.DIRECTORY)
                                        path_r = current.EnumerateDirectories().FirstOrDefault()?.FullName ?? long_path;
                                    else
                                        path_r = current.EnumerateFileSystemInfos().FirstOrDefault()?.FullName ?? long_path;
                                    reader.completion_r = PathCheck(path_r, path_mode, true, true, out _, out _);
                                }
                            }

                            if (signal.flags.HasFlag(SIG_FLAGS_new.CHANGE))
                            {
                                var paths = type switch
                                {
                                    FS_TYPES.DIRECTORY => parent.EnumerateDirectories(),
                                    _ => parent.EnumerateFileSystemInfos(),
                                };

                                foreach (var dir in parent.EnumerateDirectories())
                                    reader.completions_v.Add(PathCheck(dir.FullName, path_mode, true, true, out _, out _));

                                if (type.HasFlag(FS_TYPES.FILE))
                                    foreach (var file in parent.EnumerateFiles())
                                        reader.completions_v.Add(PathCheck(file.FullName, path_mode, true, true, out _, out _));
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }

            return true;

        failure:
            path = null;
            reader.Stderr($"could not parse path '{path}'.");
            return false;
        }
    }
}