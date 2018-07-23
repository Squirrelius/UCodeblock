namespace UCodeblock
{
    public abstract class CodeblockItem
    {
        /// <summary>
        /// The identity of the codeblock.
        /// </summary>
        public ICodeblockIdentity Identity { get; set; }

        public void GiveIdentity () => Identity = StandardIdentity.Generate();
    }
}