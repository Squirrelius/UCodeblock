namespace UCodeblock
{
    /// <summary>
    /// Inverts a boolean value. ! in C#.
    /// </summary>
    public class NotOperator : CodeblockItem, IDynamicEvaluateableCodeblock, IEvaluateableCodeblock<bool>
    {
        public IEvaluateableCodeblock<bool> Argument { get; set; }

        public T Evaluate<T>(ICodeblockExecutionContext context)
            => (T)(object)(!Argument.Evaluate(context));

        public bool Evaluate(ICodeblockExecutionContext context)
            => !Argument.Evaluate(context);

        public override IBlockError CheckErrors()
        {
            if (Argument == null)
                return StandardBlockError.EmptyParameterError;

            return null;
        }
    }
}