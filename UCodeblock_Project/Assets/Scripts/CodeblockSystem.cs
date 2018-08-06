using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace UCodeblock
{
    /// <summary>
    /// Provides a system of executeable codeblocks.
    /// </summary>
    public class CodeblockSystem
    {
        /// <summary>
        /// The collection of codeblocks in the system. Note that not all codeblocks have to be connected!
        /// </summary>
        public CodeblockCollection Blocks { get; set; }
        /// <summary>
        /// The local variables defined the the system.
        /// </summary>
        public LocalVariableTable Variables { get; set; }

        /// <summary>
        /// Is there any error in the codeblocks that will be executed?
        /// </summary>
        public bool AnyError => Errors != null && Errors.Length > 0;
        public IBlockError[] Errors => Blocks.GetMainThreadErrors().ToArray();

        /// <summary>
        /// Creates a new instance of a codeblock system.
        /// </summary>
        public CodeblockSystem()
        {
            Blocks = new CodeblockCollection();
        }
    }
}