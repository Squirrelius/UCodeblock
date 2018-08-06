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
        string ID { get; set; }
        string FromID { get; set; }
        string ToID { get; set; }
    }
}
