using UnityEngine;

using System.Collections;

namespace UCodeblock
{
    public class DebugLogBlock : CodeblockItem, IExecuteableCodeblock
    {
        [ContentProperty(0)]
        public IDynamicEvaluateableCodeblock Message { get; set; }

        public override string Content => "Print {0}.";

        public IEnumerator Execute(ICodeblockExecutionContext context)
        {
            string content = Message?.EvaluateObject(context)?.ToString();
            Debug.Log(content);

            yield return null;
        }
    }
}