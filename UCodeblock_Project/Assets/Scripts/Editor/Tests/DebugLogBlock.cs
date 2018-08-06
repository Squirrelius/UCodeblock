using System.Collections;

namespace UCodeblock.Tests
{
    public class DebugLogBlock : CodeblockItem, IExecuteableCodeblock
    {
        public string Content { get; set; }

        public IEnumerator Execute(ICodeblockExecutionContext context)
        {
            UnityEngine.Debug.Log(Content);
            yield return null;
        }
    }
}