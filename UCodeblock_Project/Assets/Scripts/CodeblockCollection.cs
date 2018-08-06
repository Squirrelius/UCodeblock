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
        /// The codeblocks contained in the collection.
        /// </summary>
        public List<CodeblockItem> Codeblocks { get; set; }
        /// <summary>
        /// The ID of the first codeblock that will be executed in this collection.
        /// </summary>
        public string EntryID { get; set; }

        /// <summary>
        /// Creates a new instance of a codeblock collection.
        /// </summary>
        public CodeblockCollection()
        {
            Codeblocks = new List<CodeblockItem>();
        }
        /// <summary>
        /// Creates a new instance of a codeblock collection, from a given source.
        /// </summary>
        public CodeblockCollection(IEnumerable<CodeblockItem> source)
        {
            Codeblocks = new List<CodeblockItem>(source);
        }

        /// <summary>
        /// Returns the codeblock in this collection by its id. Returns null if the ID isn't found.
        /// </summary>
        public CodeblockItem this[string id]
            => Codeblocks.FirstOrDefault(c => c.Identity.ID == id);
        
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
            =>  Codeblocks.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        private void Add (object obj)
            => Codeblocks.Add(obj as CodeblockItem);
        
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