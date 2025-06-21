namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseBlock(in BoaReader reader, in ScopeNode scope, out Executor block)
        {
            if (reader.TryReadChar_match('{'))
            {
                reader.LintOpeningBraquet();

                var sub_scope = new ScopeNode(scope, true);
                var body = new BlockExecutor(this, sub_scope);
                block = body;

                while (TryParseBlock(reader, sub_scope, out Executor sub_block))
                    if (sub_block != null)
                        body.stack.Add(sub_block);

                if (reader.sig_error != null)
                {
                    block = null;
                    return false;
                }

                if (reader.TryReadChar_match('}', lint: reader.CloseBraquetLint()))
                    return true;
                else
                    reader.Stderr($"expected closing bracket '}}'.");
            }
            else if (TryParseInstruction(reader, scope, true, out block))
                return true;

            return false;
        }
    }
}