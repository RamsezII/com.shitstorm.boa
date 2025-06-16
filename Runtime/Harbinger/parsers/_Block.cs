namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseBlock(in BoaReader reader, in Executor caller, out Executor block)
        {
            if (reader.TryReadChar_match('{'))
            {
                reader.LintOpeningBraquet();

                BlockExecutor body = new(this, caller);
                block = body;

                while (TryParseBlock(reader, body, out Executor sub_block))
                    if (sub_block != null)
                        body.stack.Add(sub_block);

                if (reader.error != null)
                {
                    block = null;
                    return false;
                }

                if (reader.TryReadChar_match('}', lint: reader.CloseBraquetLint()))
                    return true;
                else
                    reader.error ??= $"expected closing bracket '}}'";
            }
            else if (TryParseInstruction(reader, caller, true, out block))
                return true;

            return false;
        }
    }
}