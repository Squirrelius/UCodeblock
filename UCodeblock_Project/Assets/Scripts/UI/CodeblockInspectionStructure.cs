using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UCodeblock.UI
{
    public class CodeblockInspectionStructure : MonoBehaviour, IInitializePotentialDragHandler
    {
        public static CodeblockInspectionStructure Instance { get { return _instance; } }
        private static CodeblockInspectionStructure _instance;

        public static float CanvasScaleFactor { get; private set; }

        public CodeblockSystem CurrentSystem { get; set; }

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(gameObject);

            // for testing only
            CurrentSystem = new CodeblockSystem();
        }
        private void Start()
        {
            CanvasScaleFactor = CalculateCanvasScaleFactor();
        }

        private float CalculateCanvasScaleFactor ()
        {
            Canvas parent = GetComponentInParent<Canvas>();
            Transform t = parent.GetComponent<RectTransform>();
            Vector3 scale = t.localScale;
            float avg = (scale.x + scale.y + scale.z) / 3f;
            return avg;
        }

        private void Update()
        {
            // for testing only
            if (Input.GetKeyDown(KeyCode.F5))
            {
                StandardContext context = new StandardContext(this) { Delay = 0.5f };
                StartCoroutine(CurrentSystem.Blocks.ExecuteCodeblocks(context));
            }
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        public UICodeblock GetOverlappingDragArea (Rect check)
        {
            return FindObjectsOfType<UICodeblock>().FirstOrDefault(block => block.GetDropRect().Overlaps(check));
        }

        public void InsertItem (UICodeblock item, UICodeblock previousSibling)
        {
            // Update the codeblock collection
            List<UICodeblock> blocks = new List<UICodeblock>() { item };
            blocks.AddRange(item.GetComponentsInChildren<UICodeblock>());
            CurrentSystem.Blocks.InsertItemRange(blocks.Select(b => b.Source).ToArray(), previousSibling.Source);

            // Update the transform hierarchy
            PlaceUnderCodeblock(item, previousSibling);

            UICodeblock[] children = previousSibling.GetComponentsInChildren<UICodeblock>().Skip(1).ToArray(); // Skip the first element, since that will be the previous sibling codeblock
            // If the previous sibling has any children, insert the item between the previous sibling and its children
            if (children.Length > 0) 
                PlaceUnderCodeblock(children.First(), item);
        }
        public void DetachItem (UICodeblock item)
        {
            // Update the codeblock collection
            List<UICodeblock> blocks = new List<UICodeblock>() { item };
            blocks.AddRange(item.GetComponentsInChildren<UICodeblock>());
            CurrentSystem.Blocks.DetachItemRange(blocks.Select(b => b.Source).ToArray());

            // Update the transform hierarchy
            item.transform.SetParent(transform);
        }

        private void PlaceUnderCodeblock (UICodeblock item, UICodeblock parent)
        {
            item.transform.SetParent(parent.transform);
            item.transform.localPosition = new Vector3(0, -parent.GetComponent<RectTransform>().sizeDelta.y);
        }
    }
}