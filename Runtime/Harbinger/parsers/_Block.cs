namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseBlock(in BoaReader reader, out Executor executor, out string error)
        {
            executor = null;
            error = null;

            if (reader.HasNext())
                if (reader.TryReadChar('{'))
                {
                    if (reader.HasNext())
                    {
                        BlockExecutor block = new(this);

                        while (TryParseBlock(reader, out Executor exe, out error))
                            block.stack.Add(exe);

                        if (error != null)
                        {
                            executor = null;
                            return false;
                        }

                        executor = block;

                        if (reader.TryReadChar('}'))
                            return true;
                        else
                            error = $"did not find closing bracket '}}'";
                    }
                }
                else if (TryParseInstruction(reader, out var instruction, out error))
                {
                    executor = instruction;
                    return true;
                }

            return false;
        }
    }
}