namespace UCodeblock
{
    public class DynamicInputOperator<T> : CodeblockItem, IDynamicEvaluateableCodeblock, IEvaluateableCodeblock<T>
    {
        public T Value { get; set; }

        public object EvaluateObject(ICodeblockExecutionContext context)
            => Value;

        public T Evaluate(ICodeblockExecutionContext context)
            => Value;
    }
}