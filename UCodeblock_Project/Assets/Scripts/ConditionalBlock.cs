using System;
using System.Collections;

namespace UCodeblock
{
    public class ConditionalBlock : CodeblockItem, IExecuteableCodeblock
    {
        public IEvaluateableCodeblock<bool> Condition { get; set; }
        public CodeblockCollection Children { get; set; }

        public ConditionalBlock ()
        {
            Children = new CodeblockCollection();
        }

        public IEnumerator Execute(ICodeblockExecutionContext context)
        {
            if (Condition.Evaluate(context))
            {
                yield return context.Source.StartCoroutine(Children.ExecuteCodeblocks(context));
            }
            yield break;
        }
    }
}