namespace _BOA_
{
    partial class Harbinger
    {
        public bool TryParseProgram(in BoaReader reader, in ScopeNode scope, out Executor executor)
        {
            executor = null;

            BlockExecutor program = new(this, scope ?? new ScopeNode(null, true));
            program.scope.SetVariable("_args_", new BoaVariable(args));

            while (TryParseBlock(reader, program.scope, out var sub_block))
                if (sub_block != null)
                    program.stack.Add(sub_block);

            if (reader.sig_error != null)
                goto failure;

            if (reader.TryPeekChar_out(out char peek, out _))
            {
                reader.Stderr($"could not parse everything ({nameof(peek)}: '{peek}').");
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