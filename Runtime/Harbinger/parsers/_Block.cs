namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseBlock(in BoaReader reader, in Executor parent, out Executor block, out string error)
        {
            if (reader.TryReadChar_match('{'))
            {
                BlockExecutor body = new(this, parent);
                block = body;

                while (TryParseBlock(reader, body, out Executor sub_block, out error))
                    if (sub_block != null)
                        body.stack.Add(sub_block);

                if (error != null)
                {
                    block = null;
                    return false;
                }

                if (reader.TryReadChar_match('}'))
                    return true;
                else
                    error ??= $"did not find closing bracket '}}'";
            }
            else if (TryParseInstruction(reader, parent, true, out block, out error))
                return true;

            return false;
        }
    }
}