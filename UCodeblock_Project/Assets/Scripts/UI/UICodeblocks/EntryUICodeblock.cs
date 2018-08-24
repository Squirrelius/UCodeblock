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
    }
}