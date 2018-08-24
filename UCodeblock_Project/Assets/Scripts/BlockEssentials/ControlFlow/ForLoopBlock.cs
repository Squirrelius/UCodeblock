using System;
using System.Collections;

namespace UCodeblock
{
    /// <summary>
    /// A for-Block. Executes the child codeblocks n times.
    /// </summary>
    public class ForLoopBlock : CodeblockItem, IExecuteableCodeblock, IControlFlowBlock
    {
        [ContentProperty(0)]
        public IEvaluateableCodeblock<int> LoopCount { get; set; }
        public CodeblockCollection Children { get; set; }

        public override string Content => "For {0} times, do:";

        public ForLoopBlock()
        {
            Children = new CodeblockCollection();
        }

        public IEnumerator Execute(ICodeblockExecutionContext context)
        {
            int count = LoopCount.Evaluate(context);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
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

                    yield return new UnityEngine.WaitForSeconds(context.Delay);
                }
            }
            yield return null;
        }

        public override IBlockError CheckErrors()
        {
            if (LoopCount == null)
                return StandardBlockError.EmptyParameterError;

            return null;
        }
    }
}