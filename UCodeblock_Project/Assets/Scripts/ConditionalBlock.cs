using System;
using System.Collections;

namespace UCodeblock
{
    /// <summary>
    /// An if-Block. Executes the child coldblocks if the condition is met.
    /// </summary>
    public class ConditionalBlock : CodeblockItem, IExecuteableCodeblock, IControlFlowBlock
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