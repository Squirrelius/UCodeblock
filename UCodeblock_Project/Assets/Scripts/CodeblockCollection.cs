using System.Collections;
using System.Collections.Generic;

namespace UCodeblock
{
    public class CodeblockCollection : IEnumerable<CodeblockItem>
    {
        public List<CodeblockItem> Codeblocks;

        public CodeblockCollection()
        {
            Codeblocks = new List<CodeblockItem>();
        }

        public IEnumerator ExecuteCodeblocks (ICodeblockExecutionContext context)
        {
            foreach (CodeblockItem item in this)
            {
                if (item is IExecuteableCodeblock)
                {
                    yield return context.Source.StartCoroutine((item as IExecuteableCodeblock).Execute(context));
                    continue;
                }

            }
            yield break;
        }

        public IEnumerator<CodeblockItem> GetEnumerator()
        {
            return Codeblocks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}