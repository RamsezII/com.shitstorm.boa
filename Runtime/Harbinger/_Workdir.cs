using System;
using System.IO;

namespace _BOA_
{
    public enum PathModes : byte
    {
        TryMaintain,
        TryLocal,
        ForceFull,
    }

    partial class Harbinger
    {
        public static readonly string app_dir = Directory.GetCurrentDirectory().Replace("\\", "/");
        public string workdir;

        //--------------------------------------------------------------------------------------------------------------

        internal void ChangeWorkdir(in string path) => workdir = PathCheck(path, PathModes.ForceFull, false, false, out _, out _);

        public string PathCheck(in string path, in PathModes path_mode, in bool check_quotes, in bool force_quotes, out bool is_rooted, out bool is_local_to_shell) => PathCheck(workdir, path, path_mode, check_quotes, force_quotes, out is_rooted, out is_local_to_shell);
        public static string PathCheck(in string workdir, in string path, in PathModes path_mode, in bool check_quotes, in bool force_quotes, out bool is_rooted, out bool is_local_to_shell)
        {
            bool empty = string.IsNullOrWhiteSpace(path);

            try
            {
                string result_path = path;

                if (empty)
                {
                    is_rooted = false;
                    is_local_to_shell = true;
                    result_path = workdir;
                }
                else
                {
                    is_rooted = Path.IsPathRooted(path);
                    if (is_rooted)
                    {
                        result_path = Path.GetFullPath(result_path).Replace("\\", "/");
                        is_local_to_shell = result_path.StartsWith(workdir, StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        is_local_to_shell = true;
                        result_path = Path.Combine(workdir, result_path);
                    }
                    result_path = Path.GetFullPath(result_path);
                }

                switch (path_mode)
                {
                    case PathModes.TryMaintain when !is_rooted:
                    case PathModes.TryLocal:
                        if (is_local_to_shell)
                            result_path = Path.GetRelativePath(workdir, result_path);
                        break;
                }

                result_path = result_path.Replace("\\", "/");

                if (force_quotes)
                    result_path = result_path.QuoteStringSafely();
                else if (check_quotes)
                    result_path = result_path.QuotePathIfNeeded();

                return result_path;
            }
            catch
            {
                is_rooted = false;
                is_local_to_shell = false;
                return path;
            }
        }
    }
}