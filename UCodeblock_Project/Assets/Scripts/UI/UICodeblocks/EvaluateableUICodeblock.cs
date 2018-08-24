using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UCodeblock.UI
{
    /// <summary>
    /// Represents an evaluateable codeblock.
    /// </summary>
    public class EvaluateableUICodeblock : UICodeblock
    {
        public override void OnPointerDown(PointerEventData eventData)
        {
            // If the left mouse button is clicked, remove the item from its current input content.
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                CodeblockInspectionStructure.Instance.RemoveFromContent(this);
            }
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            // If the left mouse button is unclicked, check if there are any overlapping contents, and if so, insert it into one of them
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                InputContent[] contentsInDropArea = CodeblockInspectionStructure.Instance.GetContentsInDropArea(_transform.GetWorldRect());
                InputContent leftmostInput = contentsInDropArea.OrderBy(c => c.transform.position.x).FirstOrDefault();

                if (leftmostInput != null)
                {
                    if (leftmostInput.AllowBlockDrop(Source))
                    {
                        CodeblockInspectionStructure.Instance.InsertIntoInputContent(this, leftmostInput);
                    }
                }
            }
        }
    }
}