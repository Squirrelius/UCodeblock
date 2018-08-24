using System.Collections;

namespace UCodeblock
{
    /// <summary>
    /// Used to break out of the execution of a loop (for, while, while-true).
    /// </summary>
    public sealed class BreakCodeblock : CodeblockItem, IExecuteableCodeblock
    {
        public override string Content => "Break.";

        public IEnumerator Execute(ICodeblockExecutionContext context)
        {
            // Try to break out of the top level coroutine
            if (context.LoopModule.HasLoopCoroutine)
            {
                context.LoopModule.CancelTopCoroutine(LoopCancelReason.Break, context.Source);
            }
            
            yield return null;
        }
    }
}