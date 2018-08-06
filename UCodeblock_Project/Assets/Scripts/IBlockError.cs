namespace UCodeblock
{
    /// <summary>
    /// An error that can occurr in a <see cref="CodeblockSystem"/>.
    /// </summary>
    public interface IBlockError
    {
        bool IsError { get; set; }
    }
}