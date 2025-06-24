using _ARK_;
using System.IO;
using UnityEngine;

namespace _BOA_
{
    partial class Shell
    {
        public string workdir;

        //--------------------------------------------------------------------------------------------------------------

        void AwakeWorkDir()
        {
            if (Application.isEditor)
                workdir = Directory.GetParent(Application.dataPath).FullName;
            else
                workdir = NUCLEOR.home_path;
            workdir = Harbinger.PathCheck(Harbinger.app_dir, workdir, PathModes.ForceFull, false, false, out _, out _);
        }

        //--------------------------------------------------------------------------------------------------------------

        void RefreshShellPrefixe()
        {
            string workdir = Harbinger.PathCheck(Harbinger.app_dir, this.workdir, PathModes.TryLocal, false, false, out _, out _);

            if (Util.Equals_path(workdir, Harbinger.app_dir))
                workdir = "~";
            else
                workdir = workdir.TrimEnd('/');

            string prefixe_text = $"{ArkMachine.user_name.Value}:{workdir}$ ";
            string prefixe_lint = $"{ArkMachine.user_name.Value.SetColor("#73CC26")}:{workdir.SetColor("#73B2D9")}$ ";

            shell_status = new Contract.Status(Contract.Status.States.WAIT_FOR_STDIN, prefixe_text: prefixe_text, prefixe_lint: prefixe_lint);
            current_status = shell_status;
        }

    }
}