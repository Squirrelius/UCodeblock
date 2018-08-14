using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace UCodeblock
{
    /// <summary>
    /// Provides a holder for a collection of <see cref="CodeblockItem"/>s.
    /// </summary>
    public class CodeblockCollection : IEnumerable<CodeblockItem>
    {
        /// <summary>
        /// The codeblocks contained in the collection, indexable by their identity ID.
        /// </summary>
        public Dictionary<string, CodeblockItem> Codeblocks { get; set; }
        /// <summary>
        /// The ID of the first codeblock that will be executed in this collection.
        /// </summary>
        public string EntryID { get; set; }

        /// <summary>
        /// Creates a new instance of a codeblock collection.
        /// </summary>
        public CodeblockCollection()
        {
            Codeblocks = new Dictionary<string, CodeblockItem>();
        }
        /// <summary>
        /// Creates a new instance of a codeblock collection, from a given source.
        /// </summary>
        public CodeblockCollection(IEnumerable<CodeblockItem> source)
        {
            Codeblocks = source.ToDictionary(item => item.Identity.ID, item => item);
        }

        /// <summary>
        /// Returns the codeblock in this collection by its id. Returns null if the ID isn't found.
        /// </summary>
        public CodeblockItem this[string id]
            => Codeblocks.ContainsKey(id) ? Codeblocks[id] : null;
        
        /// <summary>
        /// Starts the execution of the codeblocks in a coroutine.
        /// </summary>
        /// <param name="context">The context in which the codeblocks should be executed.</param>
        public IEnumerator ExecuteCodeblocks(ICodeblockExecutionContext context)
        {
            Queue<CodeblockItem> chain = BuildCodeblockChain();

            while (chain.Count > 0 && chain.Peek() != null)
            {
                CodeblockItem item = chain.Dequeue();
                if (item is IExecuteableCodeblock)
                {
                    yield return context.Source.StartCoroutine((item as IExecuteableCodeblock).Execute(context));
                    yield return new UnityEngine.WaitForSeconds(context.Delay);
                }
                yield return null;
            }
        }

        /// <summary>
        /// Generates the chain of codeblocks that will be executed.
        /// </summary>
        private Queue<CodeblockItem> BuildCodeblockChain()
        {
            Queue<CodeblockItem> items = new Queue<CodeblockItem>();
            
            CodeblockItem item = null;
            string targetId = EntryID;
            while ((item = this[targetId]) != null)
            {
                items.Enqueue(item);
                targetId = item.Identity.ToID;
            }

            return items;
        }

        public IEnumerator<CodeblockItem> GetEnumerator()
            =>  Codeblocks.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        private void Add(object obj)
            => Add(obj as CodeblockItem);
        private void Add(CodeblockItem item)
        {
            if (!Codeblocks.ContainsKey(item.Identity.ID))
                Codeblocks.Add(item.Identity.ID, item);
        }
        private void AddRange(CodeblockItem[] items)
        {
            foreach (CodeblockItem item in items)
                Add(item);
        }
        
        /// <summary>
        /// Inserts an item after a previous codeblock.
        /// </summary>
        public void InsertItem(CodeblockItem item, CodeblockItem prevSibling)
        {
            InsertItemRange(new CodeblockItem[] { item }, prevSibling);
        }
        /// <summary>
        /// Inserts a range of items after a previous codeblock.
        /// </summary>
        public void InsertItemRange(CodeblockItem[] items, CodeblockItem prevSibling)
        {
            // Check if there is a previous sibling:
            // - if yes: Get the ID of it, and insert the item range between the previous sibling and the next sibling
            // - if not: Set the range as the entry, and prepend the range to the previous entry block (if there)

            string nextId = (prevSibling == null ? EntryID : prevSibling.Identity.ToID);
            CodeblockItem nextSibling = (string.IsNullOrEmpty(nextId) ? null : this[nextId]);

            InsertItemRangeBetween(items, prevSibling, nextSibling);
        }
        /// <summary>
        /// Inserts a range of items between two codeblocks.
        /// </summary>
        private void InsertItemRangeBetween (CodeblockItem[] items, CodeblockItem prevSibling, CodeblockItem nextSibling)
        {
            // Add the item to the collection
            AddRange(items);

            CodeblockItem first = items.First(),
                          last = items.Last();

            if (prevSibling == null)
            {
                // There was no previous sibling passed. Set this item as the first item in the collection.
                EntryID = first.Identity.ID;
                first.Identity.FromID = "";
            }
            else
            {
                prevSibling.Identity.ToID = first.Identity.ID;
                first.Identity.FromID = prevSibling.Identity.ID;
            }

            if (nextSibling == null)
            {
                // There was no next sibling passed.
                last.Identity.ToID = "";
            }
            else
            {
                nextSibling.Identity.FromID = last.Identity.ID;
                last.Identity.ToID = nextSibling.Identity.ID;
            }
        }

        /// <summary>
        /// Detaches an item from its previous sibling.
        /// </summary>
        public void DetachItem (CodeblockItem item)
        {
            DetachItemRange(new CodeblockItem[] { item });
        }
        /// <summary>
        /// Detaches a range of items from the previous sibling.
        /// </summary>
        public void DetachItemRange (CodeblockItem[] items)
        {
            CodeblockItem first = items.First(),
                          last = items.Last();

            // Gather the codeblock coming before the range, or null
            string prevId = first.Identity.FromID;
            CodeblockItem prev = (string.IsNullOrEmpty(prevId) ? null : this[prevId]);

            // Same as above, but with the next codeblock
            string nextId = last.Identity.ToID;
            CodeblockItem next = (string.IsNullOrEmpty(prevId) ? null : this[nextId]);

            DetachItemRangeFrom(items, prev, next);
        }
        /// <summary>
        /// Detaches a range of items from the previous sibling and the next sibling.
        /// </summary>
        private void DetachItemRangeFrom (CodeblockItem[] items, CodeblockItem prevSibling, CodeblockItem nextSibling)
        {
            CodeblockItem first = items.First(),
                          last = items.Last();
            
            if (prevSibling == null && first.Identity.ID == EntryID)
            {
                // The first item is the entry item, so reset the entry id
                EntryID = "";
            }
            if (prevSibling != null)
            {
                // Detach the first item from the previous sibling
                prevSibling.Identity.ToID = "";
                first.Identity.FromID = "";
            }

            if (nextSibling != null)
            {
                // Detach the last item from the next sibling
                nextSibling.Identity.FromID = "";
                last.Identity.ToID = "";
            }
        }

        /// <summary>
        /// Gathers all errors in the codeblocks that will be executed.
        /// </summary>
        public IEnumerable<IBlockError> GetMainThreadErrors()
        {
            return GatherErrorsRecursively(this);
        }
        private static IEnumerable<IBlockError> GatherErrorsRecursively (CodeblockCollection collection)
        {
            foreach (CodeblockItem item in collection)
            {
                IBlockError error = item.CheckErrors();
                if (error != null)
                    yield return item.CheckErrors();

                if (item is IControlFlowBlock)
                {
                    var suberrors = GatherErrorsRecursively((item as IControlFlowBlock).Children);
                    foreach (var e in suberrors)
                        if (e != null)
                            yield return error;
                }
            }
        }
    }
}