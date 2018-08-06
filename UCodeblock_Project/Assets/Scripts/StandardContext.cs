using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UCodeblock
{
    /// <summary>
    /// Provides a standard execution context.
    /// </summary>
    public class StandardContext : ICodeblockExecutionContext
    {
        public float Delay { get; set; }
        public MonoBehaviour Source { get; set; }
        public LoopOperationModule LoopModule { get; set; }

        public StandardContext(MonoBehaviour source)
        {
            Delay = 0;
            Source = source;
            LoopModule = new LoopOperationModule();
        }
    }
}