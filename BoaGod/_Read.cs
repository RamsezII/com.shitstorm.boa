using _TERMINAL_;
using System.IO;
using System;
using UnityEngine;

namespace _BOA_
{
    partial class BoaGod
    {
        void OnCmdReadScript(in LineParser line)
        {
            string path = line.ReadAsPath();
            if (line.IsCplThis)
                line.CplPath(line, path, out _);
            else if (line.IsExec)
                try
                {
                    FileInfo file = new(path);
                    if (file.Exists)
                        new BoaParser(null, file.FullName, line.ReadAll(), null);
                    else
                        Debug.LogWarning($"File not found: \"{file.FullName}\"", this);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
        }
    }
}