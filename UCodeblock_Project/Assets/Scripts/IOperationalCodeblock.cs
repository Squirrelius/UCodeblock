using System;

namespace UCodeblock
{
    /// <summary>
    /// Defines a codeblock that evaluates an operation between two <see cref="IDynamicEvaluateableCodeblock"/>s.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IOperationalCodeblock<T>
    {
        Type[] AllowedOperandTypes { get; }
        IDynamicEvaluateableCodeblock Left { get; set; }
        IDynamicEvaluateableCodeblock Right { get; set; }
    }
}