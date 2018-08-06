using System;

namespace UCodeblock
{
    interface IOperationalCodeblock<T>
    {
        Type[] AllowedOperandTypes { get; }
        IDynamicEvaluateableCodeblock Left { get; set; }
        IDynamicEvaluateableCodeblock Right { get; set; }
    }
}