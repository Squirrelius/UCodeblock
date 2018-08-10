namespace UCodeblock
{
    /// <summary>
    /// Inverts a boolean value. ! in C#.
    /// </summary>
    public class NotOperator : CodeblockItem, IDynamicEvaluateableCodeblock, IEvaluateableCodeblock<bool>
    {
        public IEvaluateableCodeblock<bool> Argument { get; set; }

        public object EvaluateObject(ICodeblockExecutionContext context)
            => !Argument.Evaluate(context);

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