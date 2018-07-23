using System;
using System.Collections;

namespace UCodeblock
{
    public class ForLoopBlock : CodeblockItem, IExecuteableCodeblock, IControlFlowBlock
    {
        public IEvaluateableCodeblock<int> LoopCount { get; set; }
        public CodeblockCollection Children { get; set; }

        public ForLoopBlock()
        {
            Children = new CodeblockCollection();
        }

        public IEnumerator Execute(ICodeblockExecutionContext context)
        {
            int count = LoopCount.Evaluate(context);
            for (int i = 0; i < count; i++)
            {
                yield return context.Source.StartCoroutine(Children.ExecuteCodeblocks(context));
                yield return null;
            }
            yield break;
        }
    }
}