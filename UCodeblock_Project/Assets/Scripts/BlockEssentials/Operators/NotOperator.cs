namespace UCodeblock
{
    /// <summary>
    /// Inverts a boolean value. ! in C#.
    /// </summary>
    public class NotOperator : CodeblockItem, IDynamicEvaluateableCodeblock, IEvaluateableCodeblock<bool>
    {
        [ContentProperty(0)]
        public IEvaluateableCodeblock<bool> Argument { get; set; }

        public override string Content => "Not {0}";

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