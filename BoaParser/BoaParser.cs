using System;
using System.Collections;
using System.Collections.Generic;
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
            Sleep,
            Var,
            Int,
            Float,
            Exec,
            _last_,
        }

        static int _id;
        const bool logGarbageCollection = true;
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

        public BoaParser(in BoaParser parser, in string path, string args, in Action onDone)
        {
            id = ++_id;
            this.path = path;
            this.onDone = onDone;

            List<string> list = new();
            while ((parser ?? this).TryReadToken(args, out string token, out args))
                list.Add(token);
            this.args = list.ToArray();

            Debug.Log($"{this} created ({nameof(path)}: {path}) (args: {this.args.ToLog()})".ToSubLog());
            BoaGod.instance.StartCoroutine(EReadAndExecute());
        }

        ~BoaParser()
        {
            if (logGarbageCollection)
                Debug.Log($"{this} destroyed ({path})".ToSubLog());
        }

        //--------------------------------------------------------------------------------------------------------------

        public IEnumerator EReadAndExecute()
        {
            Debug.Log($"{this}.{nameof(EReadAndExecute)}: \"{path}\"".ToSubLog());
            bool error = false;
            using StreamReader file = new(path, Encoding.UTF8);
            while (!error && !file.EndOfStream)
            {
                string line = file.ReadLine();
                if (!line.StartsWith("#", StringComparison.OrdinalIgnoreCase) && !line.StartsWith("//", StringComparison.OrdinalIgnoreCase))
                {
                    string token;

                    if (line.TryReadWord(out string arg0, out string newline))
                        if (Enum.TryParse(arg0, true, out Commands code) && code < Commands._last_)
                            switch (code)
                            {
                                case Commands.Log:
                                    if (TryReadToken(newline, out token, out newline))
                                        Debug.Log(token);
                                    break;

                                case Commands.Sleep:
                                    yield return OnCmdWait(newline);
                                    break;

                                case Commands.Var:
                                    TryDeclareVariable(newline, out _);
                                    break;

                                case Commands.Int:
                                    if (TryReadToken(newline, out token, out newline))
                                        if (TryComputeInt(token, out int _int))
                                            Debug.Log($"{this} computed int: {_int}");
                                    break;

                                case Commands.Float:
                                    if (TryReadToken(newline, out token, out newline))
                                        if (TryComputeFloat(token, out float _float))
                                            Debug.Log($"{this} computed float: {_float}");
                                    break;

                                case Commands.Exec:
                                    if (TryReadToken(newline, out token, out newline))
                                        new BoaParser(this, token, newline, null);
                                    break;

                                default:
                                    Debug.LogWarning($"{this} does not implement command: \"{arg0}\"");
                                    error = true;
                                    break;
                            }
                        else
                        {
                            error = true;
                            Debug.LogWarning($"{this} does not recognize command: \"{arg0}\"");
                        }
                }
            }

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