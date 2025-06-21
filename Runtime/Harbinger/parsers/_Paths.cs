using _UTIL_;
using System.IO;

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

            string long_path = shell.PathCheck(path, PathModes.ForceFull, out bool is_rooted, out bool is_local_to_shell);
            DirectoryInfo parent = Directory.GetParent(long_path);

            if (parent != null && parent.Exists)
                if (signal.flags.HasFlag(SIG_FLAGS_new.CHANGE))
                    if (reader.IsOnCursor())
                    {
                        var paths = type switch
                        {
                            FS_TYPES.DIRECTORY => parent.EnumerateDirectories(),
                            _ => parent.EnumerateFileSystemInfos(),
                        };

                        foreach (var fsi in paths)
                            reader.completions.Add(shell.PathCheck(fsi.FullName, is_rooted ? PathModes.ForceFull : PathModes.TryLocal));
                    }

            return true;
        }
    }
}