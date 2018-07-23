namespace UCodeblock
{
    public interface IEvaluateableCodeblock<T>
    {
        T Evaluate(ICodeblockExecutionContext context);
    }
}