using System.Collections;

namespace UCodeblock
{
    public interface IExecuteableCodeblock
    {
        IEnumerator Execute(ICodeblockExecutionContext context);
    }
}