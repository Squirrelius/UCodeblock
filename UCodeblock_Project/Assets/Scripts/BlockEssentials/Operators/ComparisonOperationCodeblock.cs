using System;

namespace UCodeblock
{
    /// <summary>
    /// Provides standard comparison operations.
    /// </summary>
    public enum ComparisonOperation
    {
        /// <summary>
        /// Checks if the left value and the right value are identical.
        /// </summary>
        [CustomEnumName("=")]
        Equal,
        /// <summary>
        /// Checks if the left value is smaller than the right value.
        /// </summary>
        [CustomEnumName("<")]
        SmallerThan,
        /// <summary>
        /// Checks if the left value is greater than the right value.
        /// </summary>
        [CustomEnumName(">")]
        GreaterThan,
        /// <summary>
        /// Checks if the left value is smaller than or equal to the right value.
        /// </summary>
        [CustomEnumName("<=")]
        SmallerOrEqual,
        /// <summary>
        /// Checks if the left value is greater than or equal to the right value.
        /// </summary>
        [CustomEnumName(">=")]
        GreaterOrEqual
    }

    /// <summary>
    /// Provides arithmetic operations in a codeblock, such as +, -, / and *.
    /// </summary>
    public class ComparisonOperationCodeblock : CodeblockItem, IDynamicEvaluateableCodeblock, IEvaluateableCodeblock<bool>, IOperationalCodeblock<bool>
    {
        public Type[] AllowedOperandTypes
            => new Type[] { typeof(string), typeof(int), typeof(float), typeof(bool) };

        [ContentProperty(1, PreferredWidth = 50)]
        public IEvaluateableCodeblock<ComparisonOperation> Operation { get; set; }

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
            object l = Left.EvaluateObject(context),
                   r = Right.EvaluateObject(context);

            ComparisonOperation operation = Operation.Evaluate(context);

            // This requires some handling of edge cases:

            // If the arguments are of types NUMBER and STRING, try to parse the string to a number.
            // If that doesn't work, use 0 as a backup number.

            if (l is string && (r is int || r is float))
            {
                float val = 0;
                float.TryParse(l as string, out val);
                l = val;
            }
            else if (r is string && (l is int || l is float))
            {
                float val = 0;
                float.TryParse(r as string, out val);
                r = val;
            }

            // If the arguments L and R are both numbers, apply default logic operators to them.

            if ((l is int || l is float) && (r is int || r is float))
            {
                float lhs = 0, rhs = 0;

                if (l is int) lhs = (int)l;
                if (l is float) lhs = (float)l;

                if (r is int) rhs = (int)r;
                if (r is float) rhs = (float)r;

                switch (operation)
                {
                    case ComparisonOperation.Equal:
                        return UnityEngine.Mathf.Approximately(lhs, rhs);
                    case ComparisonOperation.SmallerThan:
                        return lhs < rhs;
                    case ComparisonOperation.GreaterThan:
                        return lhs > rhs;
                    case ComparisonOperation.SmallerOrEqual:
                        return lhs <= rhs;
                    case ComparisonOperation.GreaterOrEqual:
                        return lhs >= rhs;

                    default: throw new Exception($"Invalid comparison operation selected. (ID: { (int)operation })");
                }
            }
            
            // If the arguments are of type STRING and STRING, you can only check if they are equal.
            // Any other logic operation should fail.

            if (l is string && r is string)
            {
                if (operation == ComparisonOperation.Equal)
                    return ((l as string) == (r as string));
                else
                    throw new Exception($"When comparing two strings, no other operation than Equals should be allowed.");
            }

            // Same with booleans.

            if(l is bool && r is bool)
            {
                if (operation == ComparisonOperation.Equal)
                    return (bool)l == (bool)r;
                else
                    throw new Exception($"When comparing two booleans, no other operation than Equals should be allowed.");
            }

            throw new Exception($"Invalid comparison types.");
        }

        public override IBlockError CheckErrors()
        {
            if (Left == null || Right == null)
                return StandardBlockError.EmptyParameterError;

            return null;
        }
    }
}