using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

namespace _BOA_
{
    public class BoaParser
    {
        public enum Commands : byte
        {
            Log,
            Wait,
            _last_,
        }

        //--------------------------------------------------------------------------------------------------------------

        public IEnumerator EReadAndExecute(string path, Action onDone)
        {
            Debug.Log($"{typeof(BoaParser).FullName}.{nameof(EReadAndExecute)}: \"{path}\"");
            using StreamReader file = new(path, Encoding.UTF8);
            while (!file.EndOfStream)
                if (file.ReadLine().TryReadWord(out string arg0, out string newline))
                    if (Enum.TryParse(arg0, true, out Commands code) && code < Commands._last_)
                        switch (code)
                        {
                            case Commands.Log:
                                Debug.Log(newline);
                                break;

                            case Commands.Wait:
                                yield return OnCmdWait(newline);
                                break;

                            default:
                                Debug.LogWarning($"{typeof(BoaParser).FullName} does not implement command: \"{arg0}\"");
                                break;
                        }
                    else
                        Debug.LogWarning($"{typeof(BoaParser).FullName} does not recognize command: \"{arg0}\"");
            onDone?.Invoke();
            yield break;
        }

        IEnumerator OnCmdWait(string line)
        {
            if (float.TryParse(line, out float seconds))
            {
                Debug.Log($"{typeof(BoaParser).FullName}.{nameof(EReadAndExecute)}: will wait {seconds} seconds");
                yield return new WaitForSeconds(seconds);
                Debug.Log($"{typeof(BoaParser).FullName}.{nameof(EReadAndExecute)}: waited {seconds} seconds");
            }
            else
                Debug.LogWarning($"{typeof(BoaParser).FullName}.{nameof(EReadAndExecute)}: \"{line}\" is not a valid float");
        }
    }
}