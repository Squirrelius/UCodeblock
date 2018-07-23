namespace UCodeblock
{
    public abstract class CodeblockItem
    {
        public ICodeblockIdentity Identity { get; set; }

        public void GiveIdentity () => Identity = StandardIdentity.Generate();
    }
}