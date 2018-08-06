using System;
using System.Collections;

namespace UCodeblock
{
    /// <summary>
    /// A for-Block. Executes the child codeblocks n times.
    /// </summary>
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

        public override IBlockError CheckErrors()
        {
            IBlockError error = base.CheckErrors();

            if (error.IsError)
                return error;

            if (LoopCount == null)
                return StandardBlockError.EmptyParameterError;

            return StandardBlockError.None;
        }
    }
}