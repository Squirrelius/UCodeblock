using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCodeblock
{
    /// <summary>
    /// Provides an identity to a codeblock.
    /// </summary>
    public interface ICodeblockIdentity
    {
        /// <summary>
        /// The ID of the codeblock.
        /// </summary>
        string ID { get; }
        /// <summary>
        /// The ID of the previous sibling. Empty if no previous sibling exists.
        /// </summary>
        string FromID { get; set; }
        /// <summary>
        /// The ID of the next sibling. Empty if no next sibling exists.
        /// </summary>
        string ToID { get; set; }
    }
}
