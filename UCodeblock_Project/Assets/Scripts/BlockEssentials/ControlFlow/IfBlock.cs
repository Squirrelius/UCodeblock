using System;
using System.Collections;

namespace UCodeblock
{
    /// <summary>
    /// An if-Block. Executes the child coldblocks if the condition is met.
    /// </summary>
    public class IfBlock : CodeblockItem, IExecuteableCodeblock, IControlFlowBlock
    {
        [ContentProperty(0)]
        public IEvaluateableCodeblock<bool> Condition { get; set; }
        public CodeblockCollection Children { get; set; }

        public override string Content => "If {0} then:";

        public IfBlock()
        {
            Children = new CodeblockCollection();
        }

        public IEnumerator Execute(ICodeblockExecutionContext context)
        {
            if (Condition.Evaluate(context))
            {
                IEnumerator coroutine = Children.ExecuteCodeblocks(context);
                yield return context.Source.StartCoroutine(coroutine);
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