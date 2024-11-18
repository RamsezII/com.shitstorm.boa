using _TERMINAL_;
using UnityEngine;

namespace _BOA_
{
    partial class BoaGod
    {
        void OnCmdTestPython(in LineParser line)
        {
            string path = line.ReadAsPath();
            if(line.IsExec)
            {
                // Créer un nouveau processus pour Python
                System.Diagnostics.Process pythonProcess = new()
                {
                    StartInfo =
                    {
                        FileName = "python",
                        Arguments = path,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        CreateNoWindow = true
                    }
                };
                pythonProcess.Start();

                // Envoyer des inputs
                pythonProcess.StandardInput.WriteLine("5");
                pythonProcess.StandardInput.Flush();

                // Lire les outputs
                string result = pythonProcess.StandardOutput.ReadLine();
                Debug.Log("Résultat : " + result, this);

                pythonProcess.WaitForExit();
            }
        }
    }
}