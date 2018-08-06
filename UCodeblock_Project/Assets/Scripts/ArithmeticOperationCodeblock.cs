using System;

namespace UCodeblock
{
    public enum ArithmeticOperation
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    /// <summary>
    /// Provides arithmetic operations in a codeblock, such as +, -, / and *.
    /// </summary>
    public class ArithmeticOperationCodeblock : CodeblockItem, IEvaluateableCodeblock<float>, IOperationalCodeblock<float>
    {
        public Type[] AllowedOperandTypes
            => new Type[] { typeof(int), typeof(float) };

        public ArithmeticOperation Operation { get; set; }

        public IDynamicEvaluateableCodeblock Left { get; set; }
        public IDynamicEvaluateableCodeblock Right { get; set; }

        public float Evaluate(ICodeblockExecutionContext context)
        {
            float l = (float)Left.Evaluate<object>(context),
                  r = (float)Right.Evaluate<object>(context);

            switch (Operation)
            {
                case ArithmeticOperation.Add:
                    return l + r;
                case ArithmeticOperation.Subtract:
                    return l - r;
                case ArithmeticOperation.Multiply:
                    return l * r;
                case ArithmeticOperation.Divide:
                    return l / r;

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