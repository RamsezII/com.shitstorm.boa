namespace _BOA_
{
    partial class Harbinger
    {
        public bool TryParseProgram(in BoaReader reader, out Executor executor)
        {
            executor = null;

            BlockExecutor program = new(this, null);
            program._variables.Add("_args_", new BoaVariable(args));

            while (TryParseBlock(reader, program, out var sub_block))
                if (sub_block != null)
                    program.stack.Add(sub_block);

            if (reader.error != null)
                goto failure;

            if (reader.TryPeekChar_out(out char peek))
            {
                reader.error ??= $"could not parse everything ({nameof(peek)}: '{peek}')";
                goto failure;
            }

            executor = program;
            return true;

        failure:
            reader.LocalizeError();
            return false;
        }
    }
}