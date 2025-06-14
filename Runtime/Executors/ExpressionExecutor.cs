namespace _BOA_
{
    public abstract class ExpressionExecutor : Executor
    {
        public ContractExecutor pipe_next;

        //----------------------------------------------------------------------------------------------------------

        protected ExpressionExecutor(in Harbinger harbinger, in Executor caller) : base(harbinger, caller)
        {
        }
    }
}