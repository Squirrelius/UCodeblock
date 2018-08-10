using System;
using NUnit.Framework;
using System.Collections;

namespace UCodeblock.Tests
{
    /// <summary>
    /// Asserts if a condition is true upon execution.
    /// </summary>
    internal class AssertTestBlock : CodeblockItem, IExecuteableCodeblock
    {
        /// <summary>
        /// The condition to assert.
        /// </summary>
        public Func<bool> Condition { get; set; }

        public IEnumerator Execute(ICodeblockExecutionContext context)
        {
            Assert.True(Condition());
            yield return null;
        }
    }
}