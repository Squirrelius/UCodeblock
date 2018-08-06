namespace UCodeblock
{
    /// <summary>
    /// Indicates that the block should have control over the flow of the codeblock exection.
    /// </summary>
    public interface IControlFlowBlock
    {
        CodeblockCollection Children { get; set; }
    }
}