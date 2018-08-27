using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UCodeblock.UI
{
    /// <summary>
    /// Represents the entry codeblock.
    /// </summary>
    public class EntryUICodeblock : UICodeblock
    {
        protected override void GenerateContent()
        {
            // Apply the minimum width to the block
            Vector2 minSize = UCodeblockSettings.Instance.MinBlockSize;
            transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, minSize.x);

            base.GenerateContent();
        }

        public override Rect GetDropRect()
        {
            Rect block = _content.GetWorldRect();

            // Modify the position and size to be thinner and below the block
            block.position -= new Vector2(0, block.size.y * 0.6f);
            block.size = Vector2.Scale(block.size, new Vector2(1, 0.6f));

            return block;
        }
    }
}