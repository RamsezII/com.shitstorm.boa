using System;

namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseBlock(in BoaReader reader, in Executor parent, out BlockExecutor block, out string error)
        {
            block = new(this, parent);

            if (reader.TryReadChar_match('{'))
            {
                while (TryParseBlock(reader, block, out BlockExecutor sub_block, out error))
                    if (sub_block != null)
                        block.stack.Add(sub_block);

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
            else if (TryParseInstruction(reader, block, true, out var instr, out error))
            {
                block.stack.Add(instr);
                return true;
            }
            else if (reader.TryPeekChar_out(out char peek) && !BoaReader._empties_.Contains(peek, StringComparison.OrdinalIgnoreCase))
                error ??= $"could not parse '{peek}'";

            block = null;
            return false;
        }
    }
}