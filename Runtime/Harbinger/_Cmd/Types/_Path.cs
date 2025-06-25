using _UTIL_;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Path()
        {
            AddContract(new("path",
                min_args: 1,
                args: static exe =>
                {
                    if (exe.harbinger.TryParsePath(exe.reader, FS_TYPES.BOTH, true, out string path))
                        exe.args.Add(path);
                    else
                        exe.reader.Stderr($"Expects expression of type path.");
                },
                function: static exe => exe.harbinger.PathCheck((string)exe.args[0], PathModes.TryMaintain, false, false, out _, out _)
                ));
        }
    }
}