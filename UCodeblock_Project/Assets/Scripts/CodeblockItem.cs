using System;
using System.Linq;
using System.Reflection;

namespace UCodeblock
{
    /// <summary>
    /// The base class for every codeblock.
    /// </summary>
    public abstract class CodeblockItem
    {
        public ICodeblockIdentity Identity { get; set; } = StandardIdentity.Generate();
        public virtual string Content { get; }

        public virtual IBlockError CheckErrors ()
        {
            return null;
        }

        internal bool ImplementsInterface (Type interfaceType)
        {
            return GetType().GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == interfaceType);
        }
    }
}