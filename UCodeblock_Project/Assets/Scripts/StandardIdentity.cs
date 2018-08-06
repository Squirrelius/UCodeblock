using System;

namespace UCodeblock
{
    /// <summary>
    /// Provides a standard identity, using GUID strings, for a codeblock.
    /// </summary>
    public struct StandardIdentity : ICodeblockIdentity
    {
        /// <summary>
        /// The ID of the codeblock.
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// The ID of the previous codeblock. (Empty if none.)
        /// </summary>
        public string FromID { get; set; }
        /// <summary>
        /// The ID of the next codeblock. (Empty if none.)
        /// </summary>
        public string ToID { get; set; }

        /// <summary>
        /// Generates a new <see cref="ICodeblockIdentity"/>, with a random ID.
        /// </summary>
        public static StandardIdentity Generate()
        {
            return new StandardIdentity()
            {
                ID = new Guid().ToString(),
                FromID = "",
                ToID = ""
            };
        }
    }
}