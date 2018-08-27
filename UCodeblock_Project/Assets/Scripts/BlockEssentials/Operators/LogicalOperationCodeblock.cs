using System;

namespace UCodeblock
{
    /// <summary>
    /// Provides standard logical operations.
    /// </summary>
    public enum LogicalOperation
    {
        /// <summary>
        /// Checks if the left condition and the right condition are BOTH true.
        /// </summary>
        [CustomEnumName("&&")]
        AND,
        /// <summary>
        /// Checks if EITHER the left or the right condition is true.
        /// </summary>
        [CustomEnumName("||")]
        OR
    }

    /// <summary>
    /// Provides logical boolean operators such as AND and OR.
    /// </summary>
    public class LogicOperationCodeblock : CodeblockItem, IDynamicEvaluateableCodeblock, IEvaluateableCodeblock<bool>, IOperationalCodeblock<bool>
    {
        public Type[] AllowedOperandTypes
            => new Type[] { typeof(bool) };

        [ContentProperty(1, PreferredHeight = 50)]
        public IEvaluateableCodeblock<LogicalOperation> Operation { get; set; }

        [ContentProperty(0, PreferredWidth = 75)]
        public IDynamicEvaluateableCodeblock Left { get; set; }
        [ContentProperty(2, PreferredWidth = 75)]
        public IDynamicEvaluateableCodeblock Right { get; set; }

        public override string Content => "{0} {1} {2}";

        public object EvaluateObject(ICodeblockExecutionContext context)
        {
            return Evaluate(context);
        }

        public bool Evaluate(ICodeblockExecutionContext context)
        {
            bool l = (bool)Left.EvaluateObject(context),
                 r = (bool)Right.EvaluateObject(context);

            LogicalOperation operation = Operation.Evaluate(context);
            switch (operation)
            {
                case LogicalOperation.AND:
                    return l && r;
                case LogicalOperation.OR:
                    return l || r;

                default: throw new Exception($"Invalid logical operation selected. (ID: { (int)operation })");
            }
        }

        public override IBlockError CheckErrors()
        {
            if (Left == null || Right == null)
                return StandardBlockError.EmptyParameterError;

            return null;
        }
    }
}