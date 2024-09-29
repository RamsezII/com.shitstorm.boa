using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

namespace _BOA_
{
    public partial class BoaParser
    {
        public enum Commands : byte
        {
            Log,
            Wait,
            var,
            _last_,
        }

        static int _id;
        readonly int id;
        readonly string path;
        readonly Action onDone;
        readonly string[] args;

        public override string ToString() => $"{GetType().FullName}[{id}]";

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            _id = 0;
        }

        //--------------------------------------------------------------------------------------------------------------

        public BoaParser(in string path, in Action onDone, params string[] args)
        {
            id = ++_id;
            this.path = path;
            this.onDone = onDone;
            this.args = args;
            Debug.Log($"{this} created ({path})".ToSubLog());
            BoaGod.instance.StartCoroutine(EReadAndExecute(args));
        }

        ~BoaParser()
        {
            Debug.Log($"{this} destroyed ({path})".ToSubLog());
        }

        //--------------------------------------------------------------------------------------------------------------

        public IEnumerator EReadAndExecute(params string[] args)
        {
            Debug.Log($"{this}.{nameof(EReadAndExecute)}: \"{path}\"".ToSubLog());
            using StreamReader file = new(path, Encoding.UTF8);
            while (!file.EndOfStream)
                if (file.ReadLine().TryReadWord(out string arg0, out string newline))
                    if (Enum.TryParse(arg0, true, out Commands code) && code < Commands._last_)
                        switch (code)
                        {
                            case Commands.Log:
                                if (TryReadToken(newline, out string token, out newline))
                                    Debug.Log(token);
                                break;

                            case Commands.Wait:
                                yield return OnCmdWait(newline);
                                break;

                            case Commands.var:
                                TryDeclareVariable(newline, out _);
                                break;

                            default:
                                Debug.LogWarning($"{this} does not implement command: \"{arg0}\"");
                                break;
                        }
                    else
                        Debug.LogWarning($"{this} does not recognize command: \"{arg0}\"");
            onDone?.Invoke();
            yield break;
        }

        IEnumerator OnCmdWait(string line)
        {
            if (float.TryParse(line, out float seconds))
            {
                Debug.Log($"{this}.{nameof(EReadAndExecute)}: starts waiting {seconds} seconds".ToSubLog());
                yield return new WaitForSeconds(seconds);
                Debug.Log($"{this}.{nameof(EReadAndExecute)}: ends waiting {seconds} seconds".ToSubLog());
            }
            else
                Debug.LogWarning($"{this}.{nameof(EReadAndExecute)}: \"{line}\" is not a valid float");
        }
    }
}