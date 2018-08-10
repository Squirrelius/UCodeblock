namespace UCodeblock
{
    /// <summary>
    /// Provides a holder for dynamic input.
    /// </summary>
    /// <typeparam name="T">The type that the operator can be evaluated to.</typeparam>
    public class DynamicInputOperator<T> : CodeblockItem, IDynamicEvaluateableCodeblock, IEvaluateableCodeblock<T>
    {
        public T Value { get; set; }

        public object EvaluateObject(ICodeblockExecutionContext context)
            => Value;

        public T Evaluate(ICodeblockExecutionContext context)
            => Value;
    }
}