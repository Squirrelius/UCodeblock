using System.Linq;
using System.Collections.Generic;

namespace UCodeblock
{
    public class CodeblockSystem
    {
        public List<CodeblockCollection> Collections;

        public int BlockCount => GetAllCodeblocks().Count();

        public CodeblockSystem()
        {
            Collections = new List<CodeblockCollection>();
        }

        public CodeblockItem GetByID (string id) 
            => GetAllCodeblocks().FirstOrDefault(c => c.Identity.ID == id);

        public IEnumerable<CodeblockItem> GetAllCodeblocks()
            => Collections.SelectMany(c => c.Codeblocks);
    }
}