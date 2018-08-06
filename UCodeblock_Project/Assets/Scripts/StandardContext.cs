using UnityEngine;

namespace UCodeblock
{
    /// <summary>
    /// Provides a standard execution context.
    /// </summary>
    public class StandardContext : ICodeblockExecutionContext
    {
        public MonoBehaviour Source { get; set; }
    }
}