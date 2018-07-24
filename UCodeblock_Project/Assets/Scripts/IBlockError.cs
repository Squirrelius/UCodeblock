namespace UCodeblock
{
    public interface IBlockError
    {
        bool IsError { get; set; }
        bool ErrorRestrictsExecution { get; set; }
    }
}