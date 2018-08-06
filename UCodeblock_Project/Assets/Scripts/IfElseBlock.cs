using System;
using System.Collections;

namespace UCodeblock
{
    /// <summary>
    /// An if-else block. Executes collection A if the condition is met, otherwise collection B.
    /// </summary>
    public class IfElseBlock : CodeblockItem, IExecuteableCodeblock, IControlFlowBlock
    {
        public IEvaluateableCodeblock<bool> Condition { get; set; }
        public CodeblockCollection Children { get; set; }
        public CodeblockCollection ElseChildren { get; set; }

        public IfElseBlock()
        {
            Children = new CodeblockCollection();
            ElseChildren = new CodeblockCollection();
        }

        public IEnumerator Execute(ICodeblockExecutionContext context)
        {
            IEnumerator coroutine;

            if (Condition.Evaluate(context)) coroutine = Children.ExecuteCodeblocks(context);
            else coroutine = ElseChildren.ExecuteCodeblocks(context);

            yield return context.Source.StartCoroutine(coroutine);
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