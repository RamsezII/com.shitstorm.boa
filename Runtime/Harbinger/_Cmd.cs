using _COBRA_;
using _UTIL_;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Command.static_domain.AddAction(
                "harbinger-test",
                action: static exe =>
                {
                    string code = @"
                        x = 5;
                        if (x > 2) {
                            print ""hello!"";
                        }
                        ";

                    List<Token> tokens = Tokenize(code);
                    Parser parser = new();
                    List<Node> nodes = parser.Parse(tokens);
                    // Tu peux afficher les nodes pour debug, ou écrire un interpréteur qui exécute ça !
                });

            Command.static_domain.AddRoutine(
                "run-boa-script",
                min_args: 1,
                args: exe =>
                {
                    if (exe.line.TryReadArgument(out string script_path, out _, strict: true, path_mode: FS_TYPES.FILE))
                        exe.args.Add(script_path);
                },
                routine: ERunScript);

            static IEnumerator<CMD_STATUS> ERunScript(Command.Executor exe)
            {
                string script_path = (string)exe.args[0];
                script_path = exe.shell.PathCheck(script_path, PathModes.ForceFull);

                if (!File.Exists(script_path))
                {
                    exe.error = $"file '{script_path}' does not exist";
                    yield break;
                }

                string script_text = File.ReadAllText(script_path);

                var instructions = ParseInstructions(script_text);
            }
        }
    }
}