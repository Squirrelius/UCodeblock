namespace UCodeblock
{
    /// <summary>
    /// Indicates that the block can be evaluated to return a dynamic value.
    public interface IDynamicEvaluateableCodeblock
    {
        T Evaluate<T>(ICodeblockExecutionContext context);
    }
}