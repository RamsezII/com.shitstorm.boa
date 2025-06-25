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
                opts: static exe =>
                {
                    FS_TYPES type = FS_TYPES.BOTH;
                    if (exe.reader.TryReadChar_matches_out(out char opt, true, "fd"))
                        switch (opt)
                        {
                            case 'f':
                                type = FS_TYPES.FILE;
                                break;
                            case 'd':
                                type = FS_TYPES.DIRECTORY;
                                break;
                        }
                    exe.opts.Add(nameof(FS_TYPES), type);
                },
                args: static exe =>
                {
                    FS_TYPES type = (FS_TYPES)exe.opts[nameof(FS_TYPES)];
                    if (exe.harbinger.TryParsePath(exe.reader, type, true, out string path))
                        exe.args.Add(path);
                    else
                        exe.reader.Stderr($"Expects expression of type path.");
                },
                function: static exe => exe.harbinger.PathCheck((string)exe.args[0], PathModes.TryMaintain, false, false, out _, out _)
                ));
        }
    }
}