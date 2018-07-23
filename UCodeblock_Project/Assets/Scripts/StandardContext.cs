using UnityEngine;

namespace UCodeblock
{
    public class StandardContext : ICodeblockExecutionContext
    {
        public MonoBehaviour Source { get; set; }
    }
}