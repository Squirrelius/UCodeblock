﻿using System;

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
        AND,
        /// <summary>
        /// Checks if EITHER the left or the right condition is true.
        /// </summary>
        OR
    }

    /// <summary>
    /// Provides logical boolean operators such as AND and OR.
    /// </summary>
    public class LogicOperationCodeblock : CodeblockItem, IEvaluateableCodeblock<bool>, IOperationalCodeblock<bool>
    {
        public Type[] AllowedOperandTypes
            => new Type[] { typeof(bool) };

        public LogicalOperation Operation { get; set; }

        public IDynamicEvaluateableCodeblock Left { get; set; }
        public IDynamicEvaluateableCodeblock Right { get; set; }

        public bool Evaluate(ICodeblockExecutionContext context)
        {
            bool l = (bool)Left.EvaluateObject(context),
                 r = (bool)Right.EvaluateObject(context);

            switch (Operation)
            {
                case LogicalOperation.AND:
                    return l && r;
                case LogicalOperation.OR:
                    return l || r;

                default: throw new Exception($"Invalid logical operation selected. (ID: { (int)Operation })");
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