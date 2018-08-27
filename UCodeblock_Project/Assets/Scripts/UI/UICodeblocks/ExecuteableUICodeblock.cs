using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UCodeblock.UI
{
    /// <summary>
    /// Represents an executeable codeblock.
    /// </summary>
    public class ExecuteableUICodeblock : UICodeblock
    {
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                CodeblockInspectionStructure.Instance.DetachItem(this);
            }
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                UICodeblock[] blocksInDropArea = CodeblockInspectionStructure.Instance.GetBlocksInDropArea(_transform.GetWorldRect());

                UICodeblock validPreviousSibling = blocksInDropArea.OrderBy(b => b.transform.position.x).FirstOrDefault(b => b.Type == UIBlockType.Executable || b.Type == UIBlockType.Entry || b.Type == UIBlockType.ControlFlow);

                if (validPreviousSibling != null)
                {
                    if (validPreviousSibling != this)
                    {
                        CodeblockInspectionStructure.Instance.InsertItem(this, validPreviousSibling);
                    }
                }
                else
                {
                    CodeblockInspectionStructure.Instance.DetachItem(this);
                }
            }
        }

        protected override void GenerateContent()
        {
            // Apply the minimum width and height to the block
            Vector2 minSize = UCodeblockSettings.Instance.MinBlockSize;

            LayoutElement layout = _content.GetComponent<LayoutElement>();
            layout.minWidth = minSize.x;
            layout.minHeight = minSize.y;

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