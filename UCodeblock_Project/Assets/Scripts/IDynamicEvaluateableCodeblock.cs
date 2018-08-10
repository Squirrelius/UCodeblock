namespace UCodeblock
{
    /// <summary>
    /// Indicates that the block can be evaluated to return a dynamic value.
    public interface IDynamicEvaluateableCodeblock
    {
        object EvaluateObject(ICodeblockExecutionContext context);
    }
}