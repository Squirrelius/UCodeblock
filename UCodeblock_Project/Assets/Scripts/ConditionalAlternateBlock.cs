using System;
using System.Collections;

namespace UCodeblock
{
    /// <summary>
    /// An if-else block. Executes collection A if the condition is met, otherwise collection B.
    /// </summary>
    public class ConditionalAlternateBlock : CodeblockItem, IExecuteableCodeblock, IControlFlowBlock
    {
        public IEvaluateableCodeblock<bool> Condition { get; set; }
        public CodeblockCollection Children { get; set; }
        public CodeblockCollection AlternativeChildren { get; set; }

        public ConditionalAlternateBlock()
        {
            Children = new CodeblockCollection();
            AlternativeChildren = new CodeblockCollection();
        }

        public IEnumerator Execute(ICodeblockExecutionContext context)
        {
            if (Condition.Evaluate(context))
            {
                yield return context.Source.StartCoroutine(Children.ExecuteCodeblocks(context));
            }
            else
            {
                yield return context.Source.StartCoroutine(AlternativeChildren.ExecuteCodeblocks(context));
            }
            yield break;
        }

        public override IBlockError CheckErrors()
        {
            IBlockError error = base.CheckErrors();

            if (error.IsError)
                return error;

            if (Condition == null)
                return StandardBlockError.EmptyParameterError;

            return StandardBlockError.None;
        }
    }
}