using System.Collections;

namespace UCodeblock
{
    public sealed class BreakCodeblock : CodeblockItem, IExecuteableCodeblock
    {
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