using System;
using System.Collections;

namespace UCodeblock.Tests
{
    /// <summary>
    /// Executes an action upon execution.
    /// </summary>
    internal class ActionBlock : CodeblockItem, IExecuteableCodeblock
    {
        /// <summary>
        /// The action to execute.
        /// </summary>
        public Action Action { get; set; }

        public IEnumerator Execute(ICodeblockExecutionContext context)
        {
            Action();
            yield return null;
        }
    }
}