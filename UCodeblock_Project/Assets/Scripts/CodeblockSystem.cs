using System.Linq;
using System.Collections.Generic;

namespace UCodeblock
{
    public class CodeblockSystem
    {
        public EntryBlock Entry;
        public List<CodeblockCollection> Collections;

        public int BlockCount => GetAllCodeblocks().Count();
        public bool AnyError => GetMainThreadErrors().Count() > 0;

        public CodeblockSystem()
        {
            Entry = new EntryBlock();
            Collections = new List<CodeblockCollection>();
        }

        public CodeblockItem GetByID (string id) 
            => GetAllCodeblocks().FirstOrDefault(c => c.Identity.ID == id);

        public IEnumerable<CodeblockItem> GetAllCodeblocks()
            => Collections.SelectMany(c => c.Codeblocks);

        public CodeblockCollection GetEntryCollection()
            => Collections.FirstOrDefault(c => c.Any(i => Entry.Identity.ToID == i.Identity.ID));

        public IEnumerable<IBlockError> GetMainThreadErrors()
            => GetEntryCollection().Select(i => i.CheckErrors());
    }
}