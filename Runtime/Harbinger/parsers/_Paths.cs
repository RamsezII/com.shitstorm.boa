namespace _BOA_
{
    partial class Harbinger
    {
        public bool TryParsePath(in BoaReader reader, in ScopeNode scope, out string path)
        {
            if (reader.TryParseString(out path))
            {
                if ((signal.flags & SIG_FLAGS_new.TAB) != 0)
                    path = shell.PathCheck(path, PathModes.TryMaintain);
                return true;
            }
            else
            {
                reader.Stderr($"could not parse path '{path}'.");
                return false;
            }
        }
    }
}