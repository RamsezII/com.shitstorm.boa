using _ARK_;
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
            working_dir = PathCheck(working_dir, PathModes.ForceFull);
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
            string referent_dir = PathCheck(Shell.referent_dir, PathModes.ForceFull);
            string working_dir = this.working_dir = PathCheck(this.working_dir, PathModes.ForceFull);

            if (Util.Equals_path(working_dir, referent_dir))
                working_dir = "~";
            else if (working_dir.Contains(referent_dir))
                working_dir = Path.Combine("~", Path.GetRelativePath(referent_dir, working_dir));

            working_dir = working_dir.Replace("\\", "/");

            return ($"{user_name ?? ArkMachine.user_name.Value}:{cmd_path ?? working_dir}$ ", $"{(user_name ?? ArkMachine.user_name.Value).SetColor("#73CC26")}:{(cmd_path ?? working_dir).SetColor("#73B2D9")}$ ");
        }

        public string PathCheck(in string path, in PathModes path_mode) => PathCheck(path, path_mode, out _, out _);
        public string PathCheck(in string path, in PathModes path_mode, out bool was_rooted, out bool is_local_to_shell)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    was_rooted = false;
                else
                    was_rooted = Path.IsPathRooted(path);

                string result_path = path;

                if (!was_rooted)
                    result_path = Path.GetFullPath(Path.Combine(working_dir, result_path));

                result_path = result_path.Replace("\\", "/");

                is_local_to_shell = result_path.Contains(working_dir);

                switch (path_mode)
                {
                    case PathModes.TryMaintain when !was_rooted:
                    case PathModes.TryLocal:
                        if (is_local_to_shell)
                        {
                            string local_path = Path.GetRelativePath(working_dir, result_path);
                            if (!string.IsNullOrWhiteSpace(local_path))
                                result_path = local_path.Replace("\\", "/");
                        }
                        break;
                }

                return result_path;
            }
            catch
            {
                was_rooted = false;
                is_local_to_shell = false;
                return path;
            }
        }
    }
}