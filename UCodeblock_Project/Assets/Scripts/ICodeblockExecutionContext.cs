using UnityEngine;

namespace UCodeblock
{
    /// <summary>
    /// Provides context for the execution/evaluation of a codeblock.
    /// </summary>
    public interface ICodeblockExecutionContext
    {
        MonoBehaviour Source { get; set; }
    }
}