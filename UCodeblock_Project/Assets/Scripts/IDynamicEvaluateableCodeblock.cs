namespace UCodeblock
{
    /// <summary>
    /// Indicates that the block can be evaluated to return a dynamic value.
    /// </summary>
    public interface IDynamicEvaluateableCodeblock
    {
        object EvaluateObject(ICodeblockExecutionContext context);
    }
}