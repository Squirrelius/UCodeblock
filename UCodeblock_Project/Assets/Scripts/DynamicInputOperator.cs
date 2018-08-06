namespace UCodeblock
{
    public class DynamicInputOperator<T> : CodeblockItem, IDynamicEvaluateableCodeblock, IEvaluateableCodeblock<T>
    {
        public T Value { get; set; }

        public TResult Evaluate<TResult>(ICodeblockExecutionContext context)
            => (TResult)(object)(Value);

        public T Evaluate(ICodeblockExecutionContext context)
            => Value;
    }
}