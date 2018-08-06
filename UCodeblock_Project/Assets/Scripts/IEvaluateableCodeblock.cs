namespace UCodeblock
{
    /// <summary>
    /// Indicates that the block can be evaluated to return a value of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type that the codeblock can be evaluated to.</typeparam>
    public interface IEvaluateableCodeblock<T>
    {
        T Evaluate(ICodeblockExecutionContext context);
    }
}