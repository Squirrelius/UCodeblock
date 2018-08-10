using System;

namespace UCodeblock
{
    /// <summary>
    /// Provides standard arithmetic operations.
    /// </summary>
    public enum ArithmeticOperation
    {
        /// <summary>
        /// Adds two numbers together.
        /// </summary>
        Add,
        /// <summary>
        /// Subtracts the right number from the left number.
        /// </summary>
        Subtract,
        /// <summary>
        /// Multiplies two numbers together.
        /// </summary>
        Multiply,
        /// <summary>
        /// Divides the left number by the right number;
        /// </summary>
        Divide
    }

    /// <summary>
    /// Provides arithmetic operations in a codeblock, such as +, -, / and *.
    /// </summary>
    public class ArithmeticOperationCodeblock : CodeblockItem, IDynamicEvaluateableCodeblock, IEvaluateableCodeblock<float>, IOperationalCodeblock<float>
    {
        public Type[] AllowedOperandTypes
            => new Type[] { typeof(int), typeof(float) };

        public ArithmeticOperation Operation { get; set; }

        public IDynamicEvaluateableCodeblock Left { get; set; }
        public IDynamicEvaluateableCodeblock Right { get; set; }

        public object EvaluateObject(ICodeblockExecutionContext context)
        {
            return Evaluate(context);
        }

        public float Evaluate(ICodeblockExecutionContext context)
        {
            object l = Left.EvaluateObject(context),
                   r = Right.EvaluateObject(context);

            float lhs = 0, rhs = 0;

            if (l is int) lhs = (int)l;
            if (l is float) lhs = (float)l;

            if (r is int) rhs = (int)r;
            if (r is float) rhs = (float)r;

            switch (Operation)
            {
                case ArithmeticOperation.Add:
                    return lhs + rhs;
                case ArithmeticOperation.Subtract:
                    return lhs - rhs;
                case ArithmeticOperation.Multiply:
                    return lhs * rhs;
                case ArithmeticOperation.Divide:
                    return lhs / rhs;

                default: throw new Exception($"Invalid arithmetic operation selected. (ID: { (int)Operation })");
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