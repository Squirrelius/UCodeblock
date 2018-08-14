using UnityEngine;
using System.Collections;

namespace UCodeblock
{
    public class DebugLogBlock : CodeblockItem, IExecuteableCodeblock
    {
        [ContentProperty(0)]
        public string Message { get; set; }
        [ContentProperty(1)]
        public int ID { get; set; }

        public override string Content => "Print {0}{1}.";

        public IEnumerator Execute(ICodeblockExecutionContext context)
        {
            Debug.Log(Message);
            yield return null;
        }
    }
}