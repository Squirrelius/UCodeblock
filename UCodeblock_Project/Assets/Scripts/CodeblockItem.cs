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
    }
}