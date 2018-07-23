using System;
using System.Collections;

namespace UCodeblock
{
    public class WhileLoopBlock : CodeblockItem, IExecuteableCodeblock, IControlFlowBlock
    {
        public IEvaluateableCodeblock<bool> Condition { get; set; }
        public CodeblockCollection Children { get; set; }

        public WhileLoopBlock()
        {
            Children = new CodeblockCollection();
        }

        public IEnumerator Execute(ICodeblockExecutionContext context)
        {
            while (Condition.Evaluate(context))
            {
                yield return context.Source.StartCoroutine(Children.ExecuteCodeblocks(context));
                yield return null;
            }
            yield break;
        }
    }
}