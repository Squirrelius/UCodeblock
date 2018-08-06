namespace UCodeblock
{
    /// <summary>
    /// The base class for every codeblock.
    /// </summary>
    public abstract class CodeblockItem
    {
        public ICodeblockIdentity Identity { get; set; }

        public void GiveIdentity () => Identity = StandardIdentity.Generate();

        public virtual IBlockError CheckErrors ()
        {
            if (string.IsNullOrEmpty(Identity.ID))
                return StandardBlockError.IdentityError;

            return StandardBlockError.None;
        }
    }
}