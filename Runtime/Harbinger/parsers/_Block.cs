using System;

namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseBlock(in BoaReader reader, out Executor block, out string error)
        {
            block = null;

            if (reader.TryReadChar_match('{'))
            {
                BlockExecutor body = new(this);

                while (TryParseBlock(reader, out Executor sub_block, out error))
                    if (sub_block != null)
                        body.stack.Add(sub_block);

                if (error != null)
                {
                    block = null;
                    return false;
                }

                block = body;

                if (reader.TryReadChar_match('}'))
                    return true;
                else
                    error ??= $"did not find closing bracket '}}'";
            }
            else if (TryParseInstruction(reader, true, out var instruction, out error))
            {
                block = instruction;
                return true;
            }
            else if (reader.TryPeekChar_out(out char peek) && !BoaReader._empties_.Contains(peek, StringComparison.OrdinalIgnoreCase))
                error ??= $"could not parse '{peek}'";

            return false;
        }
    }
}