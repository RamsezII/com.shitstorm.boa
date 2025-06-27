#if UNITY_EDITOR
using System.Linq;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Linter()
        {
            AddContract(new("linter", null,
                opts: static exe =>
                {
                    var dict = exe.reader.lint_theme.GetColorProperties();
                },
                args: static exe =>
                {
                    var dict = exe.reader.lint_theme.GetColorProperties();
                    string[] keys = dict.Keys.ToArray();

                    while (exe.reader.TryReadString_matches_out(out string arg, true, default, matches: keys))
                        if (dict.TryGetValue(arg, out Color color))
                            exe.reader.LintToThisPosition(color, false);
                        else
                            exe.reader.LintToThisPosition(exe.reader.lint_theme.error, false);
                }));
        }
    }
}
#endif