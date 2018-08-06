using System;

namespace UCodeblock
{
    public enum ComparisonOperation
    {
        Equal,
        SmallerThan,
        GreaterThan,
        SmallerOrEqual,
        GreaterOrEqual
    }

    /// <summary>
    /// Provides arithmetic operations in a codeblock, such as +, -, / and *.
    /// </summary>
    public class ComparisonOperationCodeblock : CodeblockItem, IEvaluateableCodeblock<bool>, IOperationalCodeblock<bool>
    {
        public Type[] AllowedOperandTypes
            => new Type[] { typeof(string), typeof(int), typeof(float), typeof(bool) };

        public ComparisonOperation Operation { get; set; }

        public IDynamicEvaluateableCodeblock Left { get; set; }
        public IDynamicEvaluateableCodeblock Right { get; set; }

        public bool Evaluate(ICodeblockExecutionContext context)
        {
            object l = Left.Evaluate<object>(context),
                   r = Right.Evaluate<object>(context);

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
                float lhs = (float)l,
                      rhs = (float)r;

                switch (Operation)
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

                    default: throw new Exception($"Invalid comparison operation selected. (ID: { (int)Operation })");
                }
            }
            
            // If the arguments are of type STRING and STRING, you can only check if they are equal.
            // Any other logic operation should fail.

            if (l is string && r is string)
            {
                if (Operation == ComparisonOperation.Equal)
                    return ((l as string) == (r as string));
                else
                    throw new Exception($"When comparing two strings, no other operation than Equals should be allowed.");
            }

            // Same with booleans.

            if(l is bool && r is bool)
            {
                if (Operation == ComparisonOperation.Equal)
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