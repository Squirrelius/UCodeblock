using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UCodeblock.UI
{
    /// <summary>
    /// Provides methods for displaying a <see cref="CodeblockSystem"/> in the UI.
    /// </summary>
    public class CodeblockInspectionStructure : MonoBehaviour, IDragHandler, IInitializePotentialDragHandler
    {
        public static CodeblockInspectionStructure Instance { get { return _instance; } }
        private static CodeblockInspectionStructure _instance;

        private Transform _handle;

        /// <summary>
        /// The system that is currently being inspected.
        /// </summary>
        public CodeblockSystem CurrentSystem { get; set; }

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            // for testing only
            CurrentSystem = new CodeblockSystem();

            _handle = transform.GetChild(0);
        }

        private void Update()
        {
            // for testing only
            if (Input.GetKeyDown(KeyCode.F5))
            {
                StandardContext context = new StandardContext(this) { Delay = 0.5f };
                StartCoroutine(CurrentSystem.Blocks.ExecuteCodeblocks(context));
            }
            if (Input.GetKeyDown(KeyCode.F6))
            {
                var chain = CurrentSystem.Blocks.BuildCodeblockChain();

                System.Text.StringBuilder builder = new System.Text.StringBuilder();

                builder.AppendLine("Entry ID: " + CurrentSystem.Blocks.EntryID);
                foreach (CodeblockItem item in chain) builder.AppendLine("> " + item.Identity.ID);

                Debug.Log(builder.ToString());
            }
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Middle) // If the left button was clicked, drag the codeblock
            {
                _handle.position += (Vector3)eventData.delta;
            }
        }

        /// <summary>
        /// Gets the <see cref="UICodeblock"/> of which the drop area overlaps the given rect.
        /// </summary>
        public UICodeblock[] GetBlocksInDropArea (Rect check)
        {
            return FindObjectsOfType<UICodeblock>().Where(block => block.GetDropRect().Overlaps(check)).ToArray();
        }

        public InputContent[] GetContentsInDropArea (Rect check)
        {
            return FindObjectsOfType<InputContent>().Where(content => content.GetDropRect().Overlaps(check)).ToArray();
        }

        /// <summary>
        /// Inserts an item into the codeblock collection, given a previous sibling as a parent. Pass null to set the item as the entry block.
        /// </summary>
        public void InsertItem (UICodeblock item, UICodeblock previousSibling)
        {
            // Check if the previous sibling has a next sibling
            var nextSiblings = previousSibling.GetComponentsInChildren<UICodeblock>().Where(b => b.Type == UIBlockType.Executable || b.Type == UIBlockType.ControlFlow);
            UICodeblock nextSibling = nextSiblings.Count() > 1 ? nextSiblings.ElementAt(1) : null;

            // Update the codeblock collection
            List<UICodeblock> blocks = new List<UICodeblock>();
            blocks.AddRange(item.GetComponentsInChildren<UICodeblock>().Where(b => b.Type == UIBlockType.Executable || b.Type == UIBlockType.ControlFlow));
            CurrentSystem.Blocks.InsertItemRange(blocks.Select(b => b.Source).ToArray(), previousSibling.Source);

            // Update the transform hierarchy
            PlaceUnderCodeblock(item, previousSibling);

            // If there is a next sibling, place it after the last child of the new item
            if (nextSibling != null)
                PlaceUnderCodeblock(nextSibling, blocks.Last());
        }
        /// <summary>
        /// Detaches an item from its parent, inside the collection and in the UI.
        /// </summary>
        public void DetachItem (UICodeblock item)
        {
            // Update the codeblock collection
            List<UICodeblock> blocks = new List<UICodeblock>() { item };
            blocks.AddRange(item.GetComponentsInChildren<UICodeblock>().Where(b => b.Type == UIBlockType.Executable || b.Type == UIBlockType.ControlFlow));
            CurrentSystem.Blocks.DetachItemRange(blocks.Select(b => b.Source).ToArray());

            // Update the transform hierarchy
            item.transform.SetParent(_handle);
            item.PositionController.TargetLocalPosition = item.PositionController.LocalPosition;
        }

        /// <summary>
        /// Places a <see cref="UICodeblock"/> under another one.
        /// </summary>
        private void PlaceUnderCodeblock (UICodeblock item, UICodeblock parent)
        {
            float yPos = -parent.GetComponent<RectTransform>().sizeDelta.y;

            item.transform.SetParent(parent.transform);
            item.PositionController.TargetLocalPosition = new Vector3(0, yPos);
        }

        public void InsertIntoInputContent (UICodeblock item, InputContent content)
        {
            item.transform.SetParent(content.DropArea);
            item.PositionController.TargetLocalPosition = new Vector3(0, 0);

            //RectTransform rt = item.GetComponent<RectTransform>();
            //rt.anchorMin = Vector2.zero;
            //rt.anchorMax = Vector2.one;
            //rt.sizeDelta = Vector2.zero;

            content.UpdateEvaluateableProperty();
        }
        public void RemoveFromContent(UICodeblock item)
        {
            item.transform.SetParent(_handle);
            item.PositionController.TargetLocalPosition = item.PositionController.LocalPosition;

            //RectTransform rt = item.GetComponent<RectTransform>();
            
            foreach (InputContent content in FindObjectsOfType<InputContent>())
            {
                content.UpdateEvaluateableProperty();
            }
        }
    }
}