namespace _BOA_
{
    internal sealed class FunctionExecutor : ContractExecutor
    {


        //----------------------------------------------------------------------------------------------------------

        public FunctionExecutor(in Harbinger harbinger, in ScopeNode scope, in FunctionContract contract, in BoaReader reader, in ExpressionExecutor pipe_previous = null, in bool parse_arguments = true) : base(harbinger, scope, contract, reader, pipe_previous, parse_arguments)
        {
        }

        //----------------------------------------------------------------------------------------------------------

    }
}