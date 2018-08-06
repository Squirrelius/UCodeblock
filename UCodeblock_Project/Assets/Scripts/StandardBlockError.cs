namespace UCodeblock
{
    /// <summary>
    /// Provides standard codeblock errors.
    /// </summary>
    public class StandardBlockError : IBlockError
    {
        public bool IsError { get; set; } = true;
        public string ErrorMessage { get; set; }

        public static StandardBlockError None
            => new StandardBlockError() { IsError = false, ErrorMessage = "" };

        public static StandardBlockError IdentityError
            => new StandardBlockError() { ErrorMessage = "The codeblock has a corrupt identity." };

        public static StandardBlockError EmptyParameterError
            => new StandardBlockError() { ErrorMessage = "The codeblock has empty parameter fields." };

        public override string ToString()
        {
            return ErrorMessage;
        }
    }
}