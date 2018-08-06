using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UCodeblock
{
    /// <summary>
    /// Provides context for the execution/evaluation of a codeblock.
    /// </summary>
    public interface ICodeblockExecutionContext
    {
        float Delay { get; set; }
        MonoBehaviour Source { get; set; }
        LoopOperationModule LoopModule { get; set; }
    }
}