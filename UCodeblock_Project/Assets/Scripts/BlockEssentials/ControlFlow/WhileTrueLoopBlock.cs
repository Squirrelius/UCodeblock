using System;
using System.Collections;

namespace UCodeblock
{
    /// <summary>
    /// A while(true)-Block. Executes its children forever.
    /// </summary>
    public class WhileTrueLoopBlock : CodeblockItem, IExecuteableCodeblock, IControlFlowBlock
    {
        public CodeblockCollection Children { get; set; }

        public WhileTrueLoopBlock()
        {
            Children = new CodeblockCollection();
        }

        public IEnumerator Execute(ICodeblockExecutionContext context)
        {
            while (true)
            {
                IEnumerator coroutine = Children.ExecuteCodeblocks(context);
                context.LoopModule.RegisterLoopCoroutine(coroutine);

                yield return context.Source.StartCoroutine(coroutine);

                // Attempt to complete the most top-level coroutine. If this fails, the method was
                // cancelled, the LastCancelReason should be used for further handling.
                if (!context.LoopModule.CompleteCoroutine(coroutine))
                {
                    var reason = context.LoopModule.LastCancelReason;

                    if (reason == LoopCancelReason.Break)
                        break;
                    if (reason == LoopCancelReason.Continue)
                        continue;
                }
                yield return null;
            }
            yield return null;
        }
    }
}