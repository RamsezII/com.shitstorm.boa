using _ARK_;
using System;
using System.IO;
using UnityEngine;

namespace _BOA_
{
    public enum PathModes : byte
    {
        TryMaintain,
        TryLocal,
        ForceFull,
    }

    partial class Shell
    {
        public static readonly string referent_dir = Directory.GetCurrentDirectory();
        public string working_dir;

        //--------------------------------------------------------------------------------------------------------------

        void AwakeWorkDir()
        {
            if (Application.isEditor)
                working_dir = Directory.GetParent(Application.dataPath).FullName;
            else
                working_dir = NUCLEOR.home_path;
            working_dir = PathCheck(working_dir, PathModes.ForceFull, false);
        }

        //--------------------------------------------------------------------------------------------------------------

        void ApplyShellPrefixe()
        {
            var (prefixe_text, prefixe_lint) = GetPrefixe();
            shell_status = new Contract.Status(Contract.Status.States.WAIT_FOR_STDIN, prefixe_text: prefixe_text, prefixe_lint: prefixe_lint);
            current_status = shell_status;
        }

        public (string text, string lint) GetPrefixe(in string user_name = null, in string cmd_path = null)
        {
            string referent_dir = PathCheck(Shell.referent_dir, PathModes.ForceFull, false);
            string working_dir = this.working_dir = PathCheck(this.working_dir, PathModes.ForceFull, false);

            if (Util.Equals_path(working_dir, referent_dir))
                working_dir = "~";
            else if (working_dir.Contains(referent_dir))
                working_dir = Path.Combine("~", Path.GetRelativePath(referent_dir, working_dir));

            working_dir = working_dir.Replace("\\", "/");

            return ($"{user_name ?? ArkMachine.user_name.Value}:{cmd_path ?? working_dir}$ ", $"{(user_name ?? ArkMachine.user_name.Value).SetColor("#73CC26")}:{(cmd_path ?? working_dir).SetColor("#73B2D9")}$ ");
        }

        internal void ChangeWorkdir(in string path) => working_dir = PathCheck(path, PathModes.ForceFull, false);

        public string PathCheck(in string path, in PathModes path_mode, in bool check_quotes) => PathCheck(path, path_mode, check_quotes, out _, out _);
        public string PathCheck(in string path, in PathModes path_mode, in bool check_quotes, out bool is_rooted, out bool is_local_to_shell)
        {
            bool empty = string.IsNullOrWhiteSpace(path);

            try
            {
                string result_path = path;

                if (empty)
                {
                    is_rooted = false;
                    is_local_to_shell = true;
                    result_path = working_dir;
                }
                else
                {
                    is_rooted = Path.IsPathRooted(path);
                    if (is_rooted)
                    {
                        result_path = Path.GetFullPath(result_path).Replace("\\", "/");
                        is_local_to_shell = result_path.StartsWith(working_dir, StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        is_local_to_shell = true;
                        result_path = Path.Combine(working_dir, result_path);
                    }
                    result_path = Path.GetFullPath(result_path);
                }

                switch (path_mode)
                {
                    case PathModes.TryMaintain when !is_rooted:
                    case PathModes.TryLocal:
                        if (is_local_to_shell)
                            result_path = "./" + Path.GetRelativePath(working_dir, result_path);
                        break;
                }

                result_path = result_path.Replace("\\", "/");

                if (Directory.Exists(result_path))
                    if (result_path[^1] != '/')
                        result_path += "/";

                if (check_quotes)
                    result_path = result_path.QuoteStringSafely();

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