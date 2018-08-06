using System.Collections;

namespace UCodeblock
{
    /// <summary>
    /// Indicates that the codeblock can be executed in a coroutine.
    /// </summary>
    public interface IExecuteableCodeblock
    {
        IEnumerator Execute(ICodeblockExecutionContext context);
    }
}