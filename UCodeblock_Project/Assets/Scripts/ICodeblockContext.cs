using UnityEngine;

namespace UCodeblock
{
    public interface ICodeblockExecutionContext
    {
        MonoBehaviour Source { get; set; }
    }
}