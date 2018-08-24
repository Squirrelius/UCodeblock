using System;
using System.Collections;

namespace UCodeblock
{
    /// <summary>
    /// A while-Block. Executes its children while the condition is met.
    /// </summary>
    public class WhileLoopBlock : CodeblockItem, IExecuteableCodeblock, IControlFlowBlock
    {
        [ContentProperty(0)]
        public IEvaluateableCodeblock<bool> Condition { get; set; }
        public CodeblockCollection Children { get; set; }

        public override string Content => "While {0} do:";

        public WhileLoopBlock()
        {
            Children = new CodeblockCollection();
        }

        public IEnumerator Execute(ICodeblockExecutionContext context)
        {
            while (Condition.Evaluate(context))
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

        public override IBlockError CheckErrors()
        {
            if (Condition == null)
                return StandardBlockError.EmptyParameterError;

            return null;
        }
    }
}