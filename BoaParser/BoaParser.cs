using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace _BOA_
{
    public static class BoaParser
    {
        public static void Execute(in string path)
        {
            Debug.Log($"{typeof(BoaParser).FullName}.{nameof(Execute)}: \"{path}\"");
            try
            {
                using StreamReader file = new(path, Encoding.UTF8);
                while (!file.EndOfStream)
                {
                    string line = file.ReadLine();
                    Debug.Log(line);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }
}