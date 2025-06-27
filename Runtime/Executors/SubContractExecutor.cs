namespace _BOA_
{
    sealed class SubContractExecutor : ContractExecutor
    {
        internal readonly ExpressionExecutor output_exe;

        //----------------------------------------------------------------------------------------------------------

        public SubContractExecutor(
            in Harbinger harbinger,
            in ScopeNode scope,
            in ExpressionExecutor output_exe,
            in SubContract contract,
            in BoaReader reader,
            in ExpressionExecutor pipe_previous = null,
            in bool parse_arguments = true
            ) : base(
                harbinger,
                scope,
                contract,
                reader,
                pipe_previous,
                parse_arguments)
        {
            this.output_exe = output_exe;
        }
    }
}