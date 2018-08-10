using System.Collections;

namespace UCodeblock
{
    /// <summary>
    /// Skips the next elements in the loop (for, while, while-true) and starts over.
    /// </summary>
    public sealed class ContinueCodeblock : CodeblockItem, IExecuteableCodeblock
    {
        public IEnumerator Execute(ICodeblockExecutionContext context)
        {
            // Try to break out of the top level coroutine
            if (context.LoopModule.HasLoopCoroutine)
            {
                context.LoopModule.CancelTopCoroutine(LoopCancelReason.Continue, context.Source);
            }

            yield return null;
        }
    }
}