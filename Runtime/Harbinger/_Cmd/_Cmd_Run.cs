using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void _Cmd_Run()
        {
            AddContract(new("run",
                max_args: 10,
                args: static exe =>
                {
                    if (exe.reader.TryReadArgument(out string path, out exe.error))
                        ;
                },
                routine: static exe =>
                {
                    return null;
                }));
        }
    }
}